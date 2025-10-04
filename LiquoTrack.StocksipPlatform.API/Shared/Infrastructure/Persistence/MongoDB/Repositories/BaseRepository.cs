using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;

/// <summary>
///     Base repository for all repositories
/// </summary>
/// <remarks>
///     This class implements the basic CRUD operations for all repositories.
///     It requires the entity type to be passed as a generic parameter.
///     It also requires the context to be passed in the constructor.
/// </remarks>
public class BaseRepository<T>(AppDbContext context) : IBaseRepository<T> where T : Entity
{
    /// <summary>
    ///    The MongoDB collection for the entity type T
    /// </summary>
    private readonly IMongoCollection<T> _collection = context.GetCollection<T>();

    /// <summary>
    ///     Adds an entity to the repository
    /// </summary>
    /// <param name="entity">Entity object to add</param>
    public async Task AddAsync(T entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        await _collection.InsertOneAsync(entity);
    }
    /// <summary>
    ///     Finds an entity by its id
    /// </summary>
    /// <param name="id">The Entity ID to Find</param>
    /// <returns>Entity object if found</returns>
    public async Task<T?> FindByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return null;
        return await _collection.Find(x => x.Id == objectId).FirstOrDefaultAsync();
    }

    /// <summary>
    ///     Updates the entity
    /// </summary>
    /// <param name="id">
    ///     The identifier of the entity to update
    /// </param>
    /// <param name="entity">
    ///     The entity object to update
    /// </param>
    public async Task UpdateAsync(string id, T entity)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            throw new ArgumentException("Invalid id format. Expected a Mongo ObjectId (24 hex chars).", nameof(id));
        if (entity is Entity baseEntity) baseEntity.UpdatedAt = DateTime.UtcNow;
        await _collection.ReplaceOneAsync(x => x.Id == objectId, entity);
    }

    /// <summary>
    ///     Updates the entity using its own Mongo _id contained in the entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    public async Task UpdateAsync(T entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));
        if (entity is Entity baseEntity) baseEntity.UpdatedAt = DateTime.UtcNow;
        await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity);
    }

    /// <summary>
    ///     Removes the entity
    /// </summary>
    /// <param name="id">The identifier of the entity to remove</param>
    public async Task DeleteAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            throw new ArgumentException("Invalid id format. Expected a Mongo ObjectId (24 hex chars).", nameof(id));
        await _collection.DeleteOneAsync(x => x.Id == objectId);
    }

    /// <summary>
    ///     Get All entities
    /// </summary>
    /// <returns>An Enumerable containing all entity objects</returns>
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }
}