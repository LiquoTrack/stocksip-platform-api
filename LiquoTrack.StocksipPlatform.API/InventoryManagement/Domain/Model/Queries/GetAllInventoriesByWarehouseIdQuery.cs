using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;

/// <summary>
///     Query to get all inventories (products) for a specific warehouse.
/// </summary>
public record GetAllInventoriesByWarehouseIdQuery(ObjectId WarehouseId);