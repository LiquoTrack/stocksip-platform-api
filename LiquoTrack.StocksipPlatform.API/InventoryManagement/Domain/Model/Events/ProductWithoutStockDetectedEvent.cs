﻿using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Events;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Events;

/// <summary>
///     This event is raised when a product with stock 0 is detected.
/// </summary>
public class ProductWithoutStockDetectedEvent(
        string accountId, 
        string productId,
        string warehouseId
    ) : IDomainEvent
{
    /// <summary>
    ///     The identifier of the account which the product with low stock belongs to.
    /// </summary>
    public string AccountId { get; private set; } = accountId;
    
    /// <summary>
    ///     The identifier of the product with low stock.
    /// </summary>
    public string ProductId { get; private set; } = productId;
    
    /// <summary>
    ///     The identifier of the warehouse where the product with low stock is located.
    /// </summary>
    public string WarehouseId { get; private set; } = warehouseId;

    /// <summary>
    ///     Date and time when the event occurred.
    /// </summary>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}