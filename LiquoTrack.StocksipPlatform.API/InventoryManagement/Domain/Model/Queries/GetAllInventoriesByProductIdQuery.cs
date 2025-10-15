using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;

/// <summary>
///     Query to get all inventories for a specific product.
/// </summary>
public record GetAllInventoriesByProductIdQuery(ObjectId ProductId);