using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;
using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;

/// <summary>
///     Interface for managing Inventory entities.
/// </summary>
public interface IInventoryRepository : IBaseRepository<Inventory>
{
    /// <summary>
    ///     Method to get an inventory item by product ID, warehouse ID and expiration date.
    /// </summary>
    /// <param name="productId">
    ///     The ID of the product to find inventory items for.
    /// </param>
    /// <param name="warehouseId">
    ///     The ID of the warehouse to find inventory items for.
    /// </param>
    /// <param name="expirationDate">
    ///     The expiration date of the inventory item to find.
    /// </param>
    /// <returns></returns>
    Task<Inventory?> GetByProductIdWarehouseIdAndExpirationDateAsync(ObjectId productId, ObjectId warehouseId, ProductExpirationDate expirationDate);
    
    /// <summary>
    ///     Method to check if an inventory item exists by product ID, warehouse ID and expiration date.
    /// </summary>
    /// <param name="productId">
    ///     The ID of the product to find inventory items for.
    /// </param>
    /// <param name="warehouseId">
    ///     The ID of the warehouse to find inventory items for.
    /// </param>
    /// <param name="expirationDate">
    ///     The expiration date of the inventory item to find.
    /// </param>
    /// <returns></returns>
    Task<bool> ExistsByProductIdWarehouseIdAndExpirationDateAsync(ObjectId productId, ObjectId warehouseId, ProductExpirationDate expirationDate);
    
    /// <summary>
    ///     Method to find all inventory items by product ID.
    /// </summary>
    /// <param name="productId">
    ///     The ID of the product to find inventory items for. 
    /// </param>
    /// <returns>
    ///     A list of inventory items for the specified product.
    /// </returns>
    Task<IEnumerable<Inventory>> FindByProductIdAsync(ObjectId productId);
    
    /// <summary>
    ///     Method to find all inventory items by warehouse ID.   
    /// </summary>
    /// <param name="warehouseId">
    ///     The ID of the warehouse to find inventory items for.
    /// </param>
    /// <returns>
    ///     A list of inventory items for the specified warehouse.
    /// </returns>
    Task<IEnumerable<Inventory>> FindByWarehouseIdAsync(ObjectId warehouseId);
}