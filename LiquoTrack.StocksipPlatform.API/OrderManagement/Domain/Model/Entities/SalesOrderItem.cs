using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Entities;

public class SalesOrderItem
{
    public ProductId ProductId { get; set; }
    public Money UnitPrice { get; set; }
    public InventoryId InventoryId { get; set; }
    public int QuantityToSell { get; set; }
    public SalesOrderItem(ProductId productId, Money unitPrice, int quantityToSell)
    {
        ProductId = productId;
        UnitPrice = unitPrice;
        QuantityToSell = quantityToSell;
    }
    public Money CalculateSubTotal() { return UnitPrice.Multiply(QuantityToSell); }
}