using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration.Namings;
using MongoDB.Driver;

namespace LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;

/// <summary>
///     Application database context.
/// </summary>
public class AppDbContext
{
    // MongoDB database instance
    private readonly IMongoDatabase _database;

    // Constructor to initialize the database context with MongoDB client and database name
    public AppDbContext(IMongoClient client, string databaseName)
    {
        if (string.IsNullOrEmpty(databaseName))
            throw new ArgumentException("Database name cannot be null or empty", nameof(databaseName));
            
        _database = client.GetDatabase(databaseName);

        // Authentication
        // Note: CreateCollection is idempotent, so it's safe to call multiple times
        try
        {
            _database.CreateCollection("users");
        }
        catch (MongoCommandException ex) when (ex.CodeName == "NamespaceExists")
        {
            // Collection already exists, which is fine
        }
    }
    
    // To access any collection with the conventions applied
    public IMongoCollection<T> GetCollection<T>() =>
        _database.GetCollectionWithConvention<T>();
}