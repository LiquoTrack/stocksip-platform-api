namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;

/// <summary>
/// Resource for adding an item to a purchase order.
/// </summary>
/// <param name="productId">The product identifier.</param>
/// <param name="unitPrice">The unit price.</param>
/// <param name="amountToPurchase">The quantity to purchase.</param>
public record AddItemToOrderResource(
    string productId,
    decimal unitPrice,
    int amountToPurchase
);