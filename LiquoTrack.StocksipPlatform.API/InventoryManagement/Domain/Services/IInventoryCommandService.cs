using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;

/// <summary>
///     Interface for handling inventory-related commands.
/// </summary>
public interface IInventoryCommandService
{
    /// <summary>
    ///     Method to handle the addition of products to a warehouse.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for adding products to a warehouse.
    /// </param>
    /// <returns>
    ///     An inventory object representing the updated inventory.
    /// </returns>
    Task<Inventory?> Handle(AddProductsToWarehouseCommand command);

    /// <summary>
    ///     Method to handle the removal of products from a warehouse.   
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for removing products from a warehouse. 
    /// </param>
    /// <returns>
    ///     The updated inventory.
    /// </returns>
    Task<Inventory?> Handle(DecreaseProductsFromWarehouseCommand command);
}