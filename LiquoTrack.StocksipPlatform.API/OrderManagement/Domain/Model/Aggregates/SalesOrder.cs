using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using CatalogId = LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects.CatalogId;
using ESalesOrderStatuses = LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.ValueObjects.ESalesOrderStatuses;
using PurchaseOrderId = LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects.PurchaseOrderId;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Aggregates;

public class SalesOrder : Entity
{
    private List<StatusHistory> _statusHistory;
    
    public string OrderCode { get; private set; }
    public PurchaseOrderId PurchaseOrderId { get; private set; }
    public ICollection<SalesOrderItem> Items { get; private set; }
    public ESalesOrderStatuses Status { get; private set; }
    public CatalogId CatalogToBuyFrom { get; private set; }
    public DateTime ReceiptDate { get; private set; }
    public DateTime CompletitionDate { get; private set; }
    public AccountId AccountId { get; set; }
    public IReadOnlyCollection<StatusHistory> StatusHistory => (_statusHistory ??= new List<StatusHistory>()).AsReadOnly();
    public DeliveryProposal? DeliveryProposal { get; private set; }

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
    /// <param name="accountId"> The account ID of the buyer </param>
    public SalesOrder(string orderCode, PurchaseOrderId purchaseOrderId, ICollection<SalesOrderItem> items, ESalesOrderStatuses status, CatalogId catalogToBuyFrom, DateTime receiptDate, DateTime completitionDate, AccountId accountId)
    {
        OrderCode = orderCode;
        PurchaseOrderId = purchaseOrderId;
        Items = items ?? new List<SalesOrderItem>();
        Status = status;
        CatalogToBuyFrom = catalogToBuyFrom;
        ReceiptDate = receiptDate;
        CompletitionDate = completitionDate;
        AccountId = accountId ?? throw new ArgumentNullException(nameof(accountId), "Account ID is required");
        _statusHistory = new List<StatusHistory>();
        _statusHistory.Add(new StatusHistory(status, "System", "Order created"));
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

    /// <summary>
    /// Update the status of the sales order
    /// </summary>
    /// <param name="newStatus">The new status</param>
    /// <param name="reason">The reason for the status change</param>
    /// <param name="changedBy">Who made the change (default: "System")</param>
    public void UpdateStatus(ESalesOrderStatuses newStatus, string reason = null, string changedBy = "System")
    {
        if (Status == newStatus) return;
        
        if (_statusHistory == null)
            _statusHistory = new List<StatusHistory>();
            
        _statusHistory.Add(new StatusHistory(Status, changedBy, reason));
        Status = newStatus;
        
        var now = DateTime.UtcNow;
        switch (newStatus)
        {
            case ESalesOrderStatuses.Received:
                ReceiptDate = now;
                break;
            case ESalesOrderStatuses.Confirmed:
                CompletitionDate = now;
                break;
            case ESalesOrderStatuses.Arrived:
                CompletitionDate = now;
                break;
        }
    }
    
    /// <summary>
    /// Gets the current status with timestamp and reason
    /// </summary>
    public StatusHistory GetCurrentStatusInfo()
    {
        return _statusHistory.OrderByDescending(sh => sh.Timestamp).FirstOrDefault()
               ?? new StatusHistory(Status, "System", "Initial status");
    }

    /// <summary>
    /// Propose a delivery schedule by supplier
    /// </summary>
    public void ProposeDelivery(DateTime proposedDate, string? notes)
    {
        DeliveryProposal = new DeliveryProposal
        {
            ProposedDate = proposedDate,
            Notes = notes,
            Status = DeliveryProposalStatus.Proposed,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Accept the current delivery proposal by the buyer
    /// </summary>
    public void AcceptDeliveryProposal(string? notes = null)
    {
        if (DeliveryProposal == null || DeliveryProposal.Status != DeliveryProposalStatus.Proposed)
            throw new InvalidOperationException("No pending delivery proposal to accept");

        DeliveryProposal.Status = DeliveryProposalStatus.Accepted;
        DeliveryProposal.RespondedAt = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(notes))
            DeliveryProposal.Notes = notes;
    }

    /// <summary>
    /// Reject the current delivery proposal by the buyer
    /// </summary>
    public void RejectDeliveryProposal(string? notes = null)
    {
        if (DeliveryProposal == null || DeliveryProposal.Status != DeliveryProposalStatus.Proposed)
            throw new InvalidOperationException("No pending delivery proposal to reject");

        DeliveryProposal.Status = DeliveryProposalStatus.Rejected;
        DeliveryProposal.RespondedAt = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(notes))
            DeliveryProposal.Notes = notes;
    }
}