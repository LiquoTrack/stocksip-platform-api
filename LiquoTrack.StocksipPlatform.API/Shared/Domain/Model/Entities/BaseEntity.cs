using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;

/// <summary>
///     This abstract class represents the base entity for all entities in the system.
///     It contains the common properties that all entities share.
/// </summary>
public abstract class Entity
{
    /// <summary>
    ///     Id of the generic entity.
    /// </summary>
    public ObjectId Id { get; } = ObjectId.GenerateNewId();
    
    /// <summary>
    ///     The date and time when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    ///     The date and time when the entity was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}