namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;

/// <summary>
///     Record for deleting a product from inventory.
/// </summary>
public record DeleteProductResource(
        string ProductId
    );