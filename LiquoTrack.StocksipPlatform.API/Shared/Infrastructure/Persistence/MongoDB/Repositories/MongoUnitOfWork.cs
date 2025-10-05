using MongoDB.Bson;
using MongoDB.Driver;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;

namespace LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;
    
public sealed class MongoUnitOfWork(IMongoClient mongoClient, IClientSessionHandle session) : IUnitOfWork, IDisposable
{
        private bool _disposed = false;
        private readonly IMongoClient _mongoClient = mongoClient ?? throw new ArgumentNullException(nameof(mongoClient));
        private IClientSessionHandle _session = session;

        public async Task CompleteAsync(CancellationToken cancellationToken = default)
        {
            if (!_disposed)
            {
                _session = await _mongoClient.StartSessionAsync(cancellationToken: cancellationToken);

                try
                {
                    // Check if the server supports transactions
                    var adminDb = _mongoClient.GetDatabase("admin");
                    var serverInfo = await adminDb.RunCommandAsync<BsonDocument>(new BsonDocument("serverStatus", 1),
                        cancellationToken: cancellationToken);

                    var supportsTransactions = serverInfo.Contains("repl") &&
                                               serverInfo["repl"].AsBsonDocument.Contains("setName");

                    if (supportsTransactions)
                    {
                        _session.StartTransaction();
                        await _session.CommitTransactionAsync(cancellationToken);
                    }
                    // No need for an else case as we're not doing any actual save operations here
                    // the repositories handle The actual save operations
                }
                catch (Exception)
                {
                    if (_session.IsInTransaction)
                    {
                        await _session.AbortTransactionAsync(cancellationToken);
                    }

                    throw;
                }
                finally
                {
                    _session?.Dispose();
                    _session = null;
                }
            }
            else
            {
                throw new ObjectDisposedException(nameof(MongoUnitOfWork));
            }
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            
            if (disposing)
            {
                // Dispose of managed resources
                _session?.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
}