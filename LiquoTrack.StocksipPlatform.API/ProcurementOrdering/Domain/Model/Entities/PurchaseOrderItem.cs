using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Entities;

public record PurchaseOrderItem
{
    public ProductId ProductId { get; init; }
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }

    public PurchaseOrderItem(ProductId productId, decimal unitPrice, int quantity)
    {
        if (unitPrice < 0)
            throw new ArgumentException("UnitPrice cannot be negative", nameof(unitPrice));
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public decimal CalculateSubTotal() => UnitPrice * Quantity;
}