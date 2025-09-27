using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Events;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Events;

/// <summary>
///     Class that represents an event for a product with low stock.
/// </summary>
public class ProductWithLowStockDetectedEvent(
        string title, 
        string message, 
        string severity, 
        string type, 
        string accountId, 
        string productId,
        string warehouseId
    ) : IEvent
{
    /// <summary>
    ///     The title of the alert to be created.
    /// </summary>
    public string Title { get; private set; } = title;
    
    /// <summary>
    ///     The message of the alert to be created.
    /// </summary>
    public string Message { get; private set; } = message;
    
    /// <summary>
    ///     The severity of the alert to be created.
    /// </summary>
    public string Severity { get; private set; } = severity;
    
    /// <summary>
    ///     The type of the alert to be created.
    /// </summary>
    public string Type { get; private set; } = type;
    
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
}