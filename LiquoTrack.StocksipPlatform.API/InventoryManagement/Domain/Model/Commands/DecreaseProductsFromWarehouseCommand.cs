using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;

/// <summary>
///     Command to decrease products from a warehouse.
/// </summary>
public record DecreaseProductsFromWarehouseCommand(ObjectId ProductId, ObjectId WarehouseId, DateTime ExpirationDate, int QuantityToDecrease);