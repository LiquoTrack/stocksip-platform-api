using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;

namespace LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories
{
    public class MongoUnitOfWork : IUnitOfWork, IDisposable
    {
        private bool _disposed = false;
        private readonly IMongoClient _mongoClient;
        private IClientSessionHandle _session;

        public MongoUnitOfWork(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient ?? throw new ArgumentNullException(nameof(mongoClient));
        }

        public async Task CompleteAsync(CancellationToken cancellationToken = default)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(MongoUnitOfWork));

            _session = await _mongoClient.StartSessionAsync(cancellationToken: cancellationToken);
            
            try
            {
                // Check if the server supports transactions
                var adminDb = _mongoClient.GetDatabase("admin");
                var serverInfo = await adminDb.RunCommandAsync<BsonDocument>(new BsonDocument("serverStatus", 1), 
                    cancellationToken: cancellationToken);
                    
                bool supportsTransactions = serverInfo.Contains("repl") && 
                                         serverInfo["repl"].AsBsonDocument.Contains("setName");

                if (supportsTransactions)
                {
                    _session.StartTransaction();
                    await _session.CommitTransactionAsync(cancellationToken);
                }
                // No need for else case as we're not doing any actual save operations here
                // The actual save operations are handled by the repositories
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

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    _session?.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
