using System.Linq;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using ESalesOrderStatuses = LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.ValueObjects.ESalesOrderStatuses;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Aggregates;

public class SalesOrder : Entity
{
    
    public string OrderCode { get; set; }
    public PurchaseOrderId PurchaseOrderId { get; set; }
    public ICollection<SalesOrderItem> Items { get; set; }
    public ESalesOrderStatuses Status { get; set; }
    public CatalogId CatalogToBuyFrom { get; set; }
    public DateTime ReceiptDate { get; set; }
    public DateTime CompletitionDate { get; set; }
    public AccountId Buyer { get; set; }

    /// <summary>
    /// This constructor is used to create a new sales order.
    /// </summary>
    /// <param name="orderCode"> The order code </param>
    /// <param name="purchaseOrderId"> The purchase order id </param>
    /// <param name="items"> The items </param>
    /// <param name="status"> The status </param>
    /// <param name="catalogToBuyFrom"> The catalog to buy from </param>
    /// <param name="receiptDate"> The receipt date </param>
    /// <param name="completitionDate"> The completition date </param>
    /// <param name="buyer"> The buyer </param>
    public SalesOrder(string orderCode, PurchaseOrderId purchaseOrderId, ICollection<SalesOrderItem> items, ESalesOrderStatuses status, CatalogId catalogToBuyFrom, DateTime receiptDate, DateTime completitionDate, AccountId buyer)
    {
        OrderCode = orderCode;
        PurchaseOrderId = purchaseOrderId;
        Items = items;
        Status = status;
        CatalogToBuyFrom = catalogToBuyFrom;
        ReceiptDate = receiptDate;
        CompletitionDate = completitionDate;
        Buyer = buyer;
    }

    /// <summary>
    /// Add an item to the sales order
    /// </summary>
    /// <param name="productId"> The product id </param>
    /// <param name="unitPrice"> The unit price </param>
    /// <param name="amountToPurchase"> The amount to purchase </param>
    public void AddItem(ProductId productId, Money unitPrice, int amountToPurchase)
    {
        Items.Add(new SalesOrderItem(productId, unitPrice, amountToPurchase));
    }

    /// <summary>
    /// Remove an item from the sales order
    /// </summary>
    /// <param name="productId"> The product id </param>
    public void RemoveItem(ProductId productId)
    {
        Items.Remove(Items.FirstOrDefault(x => x.ProductId == productId));
    }

    /// <summary>
    /// Calculate the total of the sales order
    /// </summary>
    /// <returns> The total </returns>
    public Money CalculateTotal()
    {
        var firstItem = Items?.FirstOrDefault();
        if (firstItem == null)
        {
            return new Money(0, new Currency(nameof(EValidCurrencyCodes.USD)));
        }

        var initialTotal = firstItem.CalculateSubTotal();
        return Items.Skip(1).Aggregate(initialTotal, (total, item) => total.Add(item.CalculateSubTotal()));        
    }
}