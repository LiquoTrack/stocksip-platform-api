using Cortex.Mediator;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;
using MongoDB.Driver;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Infrastructure.Persistence.MongoDB.Repositories;

/// <summary>
/// Repository for managing PurchaseOrder entities.
/// </summary>
public class PurchaseOrderRepository(AppDbContext context, IMediator mediator)
    : BaseRepository<PurchaseOrder>(context, mediator), IPurchaseOrderRepository
{
    /// <summary>
    /// The MongoDB collection for the PurchaseOrder entity.
    /// </summary>
    private readonly IMongoCollection<PurchaseOrder> _orderCollection = context.GetCollection<PurchaseOrder>();

    /// <summary>
    /// Gets a purchase order by its identifier.
    /// </summary>
    /// <param name="id">The purchase order identifier.</param>
    /// <returns>The purchase order if found, otherwise null.</returns>
    public async Task<PurchaseOrder?> GetByIdAsync(PurchaseOrderId id)
    {
        if (string.IsNullOrWhiteSpace(id.GetId))
            throw new ArgumentException("PurchaseOrderId cannot be null or empty.", nameof(id));

        return await _orderCollection
            .Find(o => o.Id.GetId == id.GetId)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Gets a purchase order by its order code.
    /// </summary>
    /// <param name="orderCode">The order code.</param>
    /// <returns>The purchase order if found, otherwise null.</returns>
    public async Task<PurchaseOrder?> GetByOrderCodeAsync(string orderCode)
    {
        if (string.IsNullOrWhiteSpace(orderCode))
            throw new ArgumentException("OrderCode cannot be null or empty.", nameof(orderCode));

        return await _orderCollection
            .Find(o => o.OrderCode == orderCode)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Gets all purchase orders for a specific buyer.
    /// </summary>
    /// <param name="buyer">The buyer account identifier.</param>
    /// <returns>A collection of purchase orders for the buyer.</returns>
    public async Task<IEnumerable<PurchaseOrder>> FindByBuyerAsync(AccountId buyer)
    {
        if (string.IsNullOrWhiteSpace(buyer.GetId))
            throw new ArgumentException("Buyer AccountId cannot be null or empty.", nameof(buyer));

        return await _orderCollection
            .Find(o => o.Buyer.GetId == buyer.GetId)
            .ToListAsync();
    }

    /// <summary>
    /// Gets all purchase orders from a specific catalog.
    /// </summary>
    /// <param name="catalogId">The catalog identifier.</param>
    /// <returns>A collection of purchase orders from the catalog.</returns>
    public async Task<IEnumerable<PurchaseOrder>> FindByCatalogAsync(CatalogId catalogId)
    {
        if (string.IsNullOrWhiteSpace(catalogId.GetId()))
            throw new ArgumentException("CatalogId cannot be null or empty.", nameof(catalogId));

        return await _orderCollection
            .Find(o => o.CatalogIdBuyFrom.GetId() == catalogId.GetId())
            .ToListAsync();
    }

    /// <summary>
    /// Checks if a purchase order exists by its order code.
    /// </summary>
    /// <param name="orderCode">The order code to check.</param>
    /// <returns>True if the order exists, otherwise false.</returns>
    public async Task<bool> ExistsByOrderCodeAsync(string orderCode)
    {
        if (string.IsNullOrWhiteSpace(orderCode))
            throw new ArgumentException("OrderCode cannot be null or empty.", nameof(orderCode));

        return await _orderCollection
            .Find(o => o.OrderCode == orderCode)
            .AnyAsync();
    }
}