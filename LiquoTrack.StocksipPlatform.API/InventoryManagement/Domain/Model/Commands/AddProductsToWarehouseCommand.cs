using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;

/// <summary>
///     Command to add stock to a product inside a warehouse (inventory).
/// </summary>
public record AddProductsToWarehouseCommand(ObjectId ProductId, ObjectId WarehouseId, DateTime ExpirationDate, int QuantityToAdd);