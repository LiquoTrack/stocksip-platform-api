using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.ACL;

public interface ISalesOrderFacade
{
    /// <summary>
    /// Creates a sales order in the Sales Ordering context using a purchase order resource.
    /// </summary>
    /// <param name="purchaseOrder">The purchase order resource to convert.</param>
    /// <returns>The ID of the newly created sales order.</returns>
    Task<string> CreateSalesOrderFromPurchaseOrderAsync(PurchaseOrderResource purchaseOrder);
    
    Task<string> ConvertPurchaseOrderToSalesOrderAsync(string purchaseOrderId);
}