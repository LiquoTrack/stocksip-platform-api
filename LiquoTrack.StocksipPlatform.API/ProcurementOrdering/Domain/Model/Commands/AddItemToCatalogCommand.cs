using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;

/// <summary>
/// Command to add an item to a catalog.
/// </summary>
/// <param name="CatalogId">The catalog identifier.</param>
/// <param name="ProductId">The product identifier.</param>
/// <param name="WarehouseId">The warehouse identifier where the product is stored.</param>
public record AddItemToCatalogCommand(
    string CatalogId,
    string ProductId,
    string WarehouseId
);