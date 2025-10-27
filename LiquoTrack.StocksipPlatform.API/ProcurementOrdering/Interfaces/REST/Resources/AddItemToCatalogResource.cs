using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;

/// <summary>
/// Resource for adding an item to a catalog.
/// </summary>
/// <param name="productId">The product identifier.</param>

public record AddItemToCatalogResource(
    string productId
);