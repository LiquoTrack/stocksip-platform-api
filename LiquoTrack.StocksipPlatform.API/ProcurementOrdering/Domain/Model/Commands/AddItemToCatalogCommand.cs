namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;

/// <summary>
/// Command to add an item to a catalog.
/// </summary>
/// <param name="catalogId">The identifier of the catalog.</param>
/// <param name="productId">The identifier of the product to add.</param>
/// <param name="amount">The price amount of the product.</param>
/// <param name="currency">The currency of the price.</param>
public record AddItemToCatalogCommand(string catalogId, string productId, decimal amount, string currency);
