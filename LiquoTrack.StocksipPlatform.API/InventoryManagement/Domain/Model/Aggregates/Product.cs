using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;

/// <summary>
///     The Product Aggregate Root entity.
/// </summary>
public class Product : IEntity
{
    /// <summary>
    ///     The unique identifier of the product.
    /// </summary>
    public ObjectId Id { get; } = ObjectId.GenerateNewId();
    
    
}