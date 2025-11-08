using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Entities;

/// <summary>
///     A entity representing a product exit from inventory.
/// </summary>
/// <param name="inventoryAffectedId">
///     The identifier of the inventory affected by the product exit.
/// </param>
/// <param name="exitReason">
///     The reason for the product exit.
/// </param>
/// <param name="outputQuantity">
///     The quantity of the product that was outputted.
/// </param>
public class ProductExit(
    ObjectId inventoryAffectedId, 
    EProductExitReasons exitReason, 
    int outputQuantity)
    : Entity
{
    /// <summary>
    /// The identifier of the inventory affected by the product exit.
    /// </summary>
    public ObjectId InventoryAffectedId { get; private set; } = inventoryAffectedId;

    /// <summary>
    /// The reason for the product exit.
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    public EProductExitReasons ExitReason { get; private set; } = exitReason;

    /// <summary>
    /// The quantity of the product that was outputted.
    /// </summary>
    public int OutputQuantity { get; private set; } = outputQuantity;
}