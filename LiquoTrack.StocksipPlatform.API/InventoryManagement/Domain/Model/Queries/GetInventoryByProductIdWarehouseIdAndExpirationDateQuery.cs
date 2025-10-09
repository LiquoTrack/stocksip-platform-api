using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;

/// <summary>
///     Query to get an inventory by product ID, warehouse ID and expiration date.
/// </summary>
public record GetInventoryByProductIdWarehouseIdAndExpirationDateQuery(ObjectId ProductId, ObjectId WarehouseId, ProductExpirationDate ExpirationDate);