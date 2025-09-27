using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;

/// <summary>
///     Command to update the minimum stock of a product.
/// </summary>
public record UpdateProductMinimumStockCommand(int NewMinimumStock);