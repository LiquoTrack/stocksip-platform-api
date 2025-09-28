namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;

/// <summary>
///     Record representing the resource for registering a new product.
/// </summary>
public record RegisterProductResource(
        string Name,
        string Type,
        string Brand,
        decimal UnitPrice,
        string Code,
        int MinimumStock,
        string ImageUrl,
        string AccountId,
        string SupplierId
    );