using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Aggregates;

/// <summary>
/// Aggregate entity representing a purchase order.
/// </summary>
public class PurchaseOrder(
    string orderCode,
    CatalogId catalogIdBuyFrom,
    AccountId buyer
) : Entity, IConfirmable
{
    /// <summary>
    /// The unique identifier of the purchase order.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public PurchaseOrderId Id { get; private set; } = new(ObjectId.GenerateNewId().ToString());

    /// <summary>
    /// The unique code of the purchase order.
    /// </summary>
    public string OrderCode { get; private set; } = ValidateOrderCode(orderCode);

    /// <summary>
    /// The list of items in the purchase order.
    /// </summary>
    public List<PurchaseOrderItem> Items { get; private set; } = new();

    /// <summary>
    /// The current status of the purchase order.
    /// </summary>
    public EOrderStatus Status { get; private set; } = EOrderStatus.Processing;

    /// <summary>
    /// The identifier of the catalog to buy from.
    /// </summary>
    public CatalogId CatalogIdBuyFrom { get; private set; } = catalogIdBuyFrom;

    /// <summary>
    /// The date when the order was generated.
    /// </summary>
    public DateTime GenerationDate { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// The date when the order was confirmed.
    /// </summary>
    public DateTime? ConfirmationDate { get; private set; }

    /// <summary>
    /// The account identifier of the buyer.
    /// </summary>
    public AccountId Buyer { get; private set; } = buyer;

    /// <summary>
    /// Indicates whether the order has been sent.
    /// </summary>
    public bool IsOrderSent { get; private set; } = false;

    /// <summary>
    /// Command constructor to create a new PurchaseOrder instance from a CreatePurchaseOrderCommand.
    /// </summary>
    /// <param name="command">
    /// The command containing the details to create a new purchase order.
    /// </param>
    public PurchaseOrder(CreatePurchaseOrderCommand command)
        : this(command.orderCode, new CatalogId(command.catalogIdBuyFrom), new AccountId(command.buyer))
    {
    }

    // Constructor for MongoDB deserialization
    public PurchaseOrder() : this(string.Empty, new CatalogId(ObjectId.GenerateNewId().ToString()), new AccountId(ObjectId.GenerateNewId().ToString()))
    {
    }

    /// <summary>
    /// Validates the order code.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// The order code cannot be null or empty.
    /// </exception>
    private static string ValidateOrderCode(string orderCode)
        => string.IsNullOrWhiteSpace(orderCode)
            ? throw new ArgumentException("The order code cannot be null or empty.", nameof(orderCode))
            : orderCode;

    /// <summary>
    /// Adds an item to the purchase order using a command.
    /// </summary>
    /// <param name="command">
    /// The command containing the item details.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Cannot add items to a non-processing order.
    /// </exception>
    public void AddItem(AddItemToOrderCommand command)
    {
        if (Status != EOrderStatus.Processing)
            throw new InvalidOperationException("Cannot add items to a non-processing order");

        var productId = new ProductId(command.productId);
        var item = new PurchaseOrderItem(productId, command.unitPrice, command.amountToPurchase);
        Items.Add(item);
    }

    /// <summary>
    /// Removes an item from the purchase order using a command.
    /// </summary>
    /// <param name="command">
    /// The command containing the product identifier to remove.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Cannot remove items from a non-processing order.
    /// </exception>
    public void RemoveItem(RemoveItemFromOrderCommand command)
    {
        if (Status != EOrderStatus.Processing)
            throw new InvalidOperationException("Cannot remove items from a non-processing order");

        var productId = new ProductId(command.productId);
        var item = Items.FirstOrDefault(i => i.ProductId.GetId == productId.GetId);
        if (item != null)
            Items.Remove(item);
    }

    /// <summary>
    /// Calculates the total amount of the purchase order.
    /// </summary>
    /// <returns>
    /// The total amount of money to be paid.
    /// </returns>
    public Money CalculateTotal()
    {
        var total = Items.Sum(item => item.CalculateSubTotal());
        return new Money(total, new Currency("USD"));
    }

    /// <summary>
    /// Sets the order status to Processing.
    /// </summary>
    public void ProcessOrder()
    {
        Status = EOrderStatus.Processing;
    }

    /// <summary>
    /// Confirms the purchase order.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Can only confirm processing orders.
    /// </exception>
    public void ConfirmOrder()
    {
        if (Status != EOrderStatus.Processing)
            throw new InvalidOperationException("Can only confirm processing orders");

        Status = EOrderStatus.Confirmed;
        ConfirmationDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the order as shipped.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Can only ship confirmed orders.
    /// </exception>
    public void ShipOrder()
    {
        if (Status != EOrderStatus.Confirmed)
            throw new InvalidOperationException("Can only ship confirmed orders");

        Status = EOrderStatus.Shipped;
    }

    /// <summary>
    /// Marks the order as received.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Can only receive shipped orders.
    /// </exception>
    public void ReceiveOrder()
    {
        if (Status != EOrderStatus.Shipped)
            throw new InvalidOperationException("Can only receive shipped orders");

        Status = EOrderStatus.Received;
    }

    /// <summary>
    /// Cancels the purchase order.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Cannot cancel received orders.
    /// </exception>
    public void CancelOrder()
    {
        if (Status == EOrderStatus.Received)
            throw new InvalidOperationException("Cannot cancel received orders");

        Status = EOrderStatus.Canceled;
    }
}