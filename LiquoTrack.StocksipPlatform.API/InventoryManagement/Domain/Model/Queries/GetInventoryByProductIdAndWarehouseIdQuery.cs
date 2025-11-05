using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;

/// <summary>
///     Query to get the inventory of a product in a warehouse.
/// </summary>
public record GetInventoryByProductIdAndWarehouseIdQuery(
    ObjectId ProductId,
    ObjectId WarehouseId);