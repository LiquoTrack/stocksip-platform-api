using Cortex.Mediator;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Persistence.MongoDB.Repositories;

/// <summary>
///     Implementation of the ProductExitRepository interface.
/// </summary>
public class ProductExitRepository(AppDbContext context, IMediator mediator)
    : BaseRepository<ProductExit>(context, mediator), IProductExitRepository
{
    /// <summary>
    ///     Collection for the ProductExit entity.
    /// </summary>
    private readonly IMongoCollection<ProductExit> _productExitCollection = context.GetCollection<ProductExit>();

    /// <summary>
    ///     The MongoDB context. 
    /// </summary>
    private readonly AppDbContext _context = context;

    /// <summary>
    ///     Retrieves all product exits for a given warehouse ID.
    /// </summary>
    /// <param name="warehouseId">
    ///     The ID of the warehouse to retrieve product exits for.
    /// </param>
    /// <returns>
    ///     A list of product exits for the specified warehouse or a blank list if no product exits are found.
    /// </returns>
    public async Task<IEnumerable<ProductExit>> GetAllByWarehouseIdAsync(ObjectId warehouseId)
    {
        var inventoryCollection = _context.GetCollection<Inventory>();
        
        var inventoryIds = await inventoryCollection
            .Find(i => i.WarehouseId == warehouseId)
            .Project(i => i.Id)
            .ToListAsync()
            .ConfigureAwait(false);
        
        if (inventoryIds == null || inventoryIds.Count == 0)
        {
            return [];
        }
        
        var productExits = await _productExitCollection
            .Find(x => inventoryIds.Contains(x.InventoryAffectedId))
            .ToListAsync()
            .ConfigureAwait(false);

        return productExits;
    }
}