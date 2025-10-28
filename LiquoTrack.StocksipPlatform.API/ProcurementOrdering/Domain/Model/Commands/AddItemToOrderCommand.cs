namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;

/// <summary>
/// Command to add an item to an existing purchase order.
/// </summary>
/// <param name="orderId">The identifier of the purchase order.</param>
/// <param name="productId">The identifier of the product to add.</param>
/// <param name="unitPrice">The unit price of the product.</param>
/// <param name="amountToPurchase">The quantity to purchase.</param>
public record AddItemToOrderCommand(string orderId, string productId, decimal unitPrice, int amountToPurchase);
