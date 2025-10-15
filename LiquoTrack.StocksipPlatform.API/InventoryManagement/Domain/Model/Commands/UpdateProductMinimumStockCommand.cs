using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;

/// <summary>
///     Command to update the minimum stock of a product.
/// </summary>
public record UpdateProductMinimumStockCommand(ObjectId ProductId, int NewMinimumStock);