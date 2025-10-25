using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Repositories;

/// <summary>
/// Repository interface for purchase order operations.
/// </summary>
public interface IPurchaseOrderRepository
{
    /// <summary>
    /// Gets a purchase order by its identifier.
    /// </summary>
    /// <param name="id">The purchase order identifier.</param>
    /// <returns>The purchase order if found, otherwise null.</returns>
    Task<PurchaseOrder?> GetByIdAsync(PurchaseOrderId id);

    /// <summary>
    /// Gets a purchase order by its order code.
    /// </summary>
    /// <param name="orderCode">The order code.</param>
    /// <returns>The purchase order if found, otherwise null.</returns>
    Task<PurchaseOrder?> GetByOrderCodeAsync(string orderCode);

    /// <summary>
    /// Gets all purchase orders.
    /// </summary>
    /// <returns>A collection of all purchase orders.</returns>
    Task<IEnumerable<PurchaseOrder>> GetAllAsync();

    /// <summary>
    /// Gets all purchase orders for a specific buyer.
    /// </summary>
    /// <param name="buyer">The buyer account identifier.</param>
    /// <returns>A collection of purchase orders for the buyer.</returns>
    Task<IEnumerable<PurchaseOrder>> GetByBuyerAsync(AccountId buyer);

    /// <summary>
    /// Gets all purchase orders from a specific catalog.
    /// </summary>
    /// <param name="catalogId">The catalog identifier.</param>
    /// <returns>A collection of purchase orders from the catalog.</returns>
    Task<IEnumerable<PurchaseOrder>> GetByCatalogAsync(CatalogId catalogId);

    /// <summary>
    /// Creates a new purchase order.
    /// </summary>
    /// <param name="order">The purchase order to create.</param>
    /// <returns>The created purchase order.</returns>
    Task<PurchaseOrder> CreateAsync(PurchaseOrder order);

    /// <summary>
    /// Updates an existing purchase order.
    /// </summary>
    /// <param name="order">The purchase order to update.</param>
    /// <returns>True if the update was successful, otherwise false.</returns>
    Task<bool> UpdateAsync(PurchaseOrder order);

    /// <summary>
    /// Deletes a purchase order.
    /// </summary>
    /// <param name="id">The purchase order identifier.</param>
    /// <returns>True if the deletion was successful, otherwise false.</returns>
    Task<bool> DeleteAsync(PurchaseOrderId id);
}