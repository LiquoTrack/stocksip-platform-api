namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Entities;

public record PurchaseOrderItem
{
    public string ProductId { get; init; }
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }

    public PurchaseOrderItem(string productId, decimal unitPrice, int quantity)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new ArgumentException("ProductId cannot be empty", nameof(productId));
        if (unitPrice < 0)
            throw new ArgumentException("UnitPrice cannot be negative", nameof(unitPrice));
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        ProductId = productId;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public decimal CalculateSubTotal() => UnitPrice * Quantity;
}