using Cortex.Mediator.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.CommandServices;

/// <summary>
///     Service to handle inventory commands
/// </summary>
/// <param name="inventoryRepository">
///     The repository for handling the Inventories in the database.
/// </param>
public class InventoryCommandService(
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository,
        IInventoryRepository inventoryRepository
    ) : IInventoryCommandService
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
    public async Task<Inventory?> Handle(AddProductsToWarehouseCommand command)
    {
        // Validate if the product exists
        var product = await productRepository.FindByIdAsync(command.ProductId.ToString())
            ?? throw new ArgumentException($"Product with ID {command.ProductId} does not exist.");
        
        // Validate if the warehouse exists
        var warehouse = await warehouseRepository.FindByIdAsync(command.WarehouseId.ToString())
            ?? throw new ArgumentException($"Warehouse with ID {command.WarehouseId} does not exist.");

        // Validate if the expiration date is provided
        if (command.ExpirationDate == null)
        {
            throw new ArgumentException("Expiration date is required when adding products with expiration date.");
        }

        // Validate if the inventory already exists
        var inventory = await inventoryRepository.GetByProductIdWarehouseIdAndExpirationDateAsync(command.ProductId,
                command.WarehouseId, command.ExpirationDate);

        // When the inventory does not exist, create it
        if (inventory == null)
        {
            var productStock = new ProductStock(command.QuantityToAdd);
            var newInventory = new Inventory(command.ProductId, command.WarehouseId, productStock, command.ExpirationDate);
            await inventoryRepository.AddAsync(newInventory);
            return newInventory;
        }
        
        // When the inventory exists, add the products to it
        inventory.AddStockToProduct(command.QuantityToAdd, product.MinimumStock.GetValue());
        await inventoryRepository.UpdateAsync(inventory.Id.ToString(), inventory);
        return inventory;
    }

    /// <summary>
    ///     Method to handle the addition of products to a warehouse without an expiration date. 
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for adding products to a warehouse.
    /// </param>
    /// <returns>
    ///     The inventory object representing the updated inventory or null if the inventory could not be updated.
    /// </returns>
    public async Task<Inventory?> Handle(AddProductsToWarehouseWithoutExpirationDateCommand command)
    {
        // Validate if the product exists
        var product = await productRepository.FindByIdAsync(command.ProductId.ToString())
                      ?? throw new ArgumentException($"Product with ID {command.ProductId} does not exist.");
        
        // Validate if the warehouse exists
        var warehouse = await warehouseRepository.FindByIdAsync(command.WarehouseId.ToString())
                        ?? throw new ArgumentException($"Warehouse with ID {command.WarehouseId} does not exist.");
        
        // Validate if the inventory already exists
        var inventory = await inventoryRepository.GetByProductIdWarehouseIdAsync(command.ProductId, command.WarehouseId);

        // When the inventory does not exist, create it
        if (inventory == null)
        {
            var productStock = new ProductStock(command.QuantityToAdd);
            var newInventory = new Inventory(command.ProductId, command.WarehouseId, productStock);
            await inventoryRepository.AddAsync(newInventory);
            return newInventory;
        }
        
        // When the inventory exists, add the products to it
        inventory.AddStockToProduct(command.QuantityToAdd, product.MinimumStock.GetValue());
        
        // Updates the inventory in the repository.
        await inventoryRepository.UpdateAsync(inventory.Id.ToString(), inventory);
        
        // Returns the updated inventory.
        return inventory;
    }

    /// <summary>
    ///     Method to handle the removal of products from a warehouse.   
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for removing products from a warehouse. 
    /// </param>
    /// <returns>
    ///     The updated inventory.
    /// </returns>
    public async Task<Inventory?> Handle(DecreaseProductsFromWarehouseCommand command)
    {
        // Checks if the product exists.
        var product = await productRepository.FindByIdAsync(command.ProductId.ToString())
                      ?? throw new ArgumentException($"Product with ID {command.ProductId} does not exist.");
        
        // Checks if the warehouse exists.
        var warehouse = await warehouseRepository.FindByIdAsync(command.WarehouseId.ToString()) 
                        ?? throw new ArgumentException($"Warehouse with ID {command.WarehouseId} does not exist.");
        
        // Checks if the inventory exists.
        var inventoryToUpdate = await inventoryRepository.GetByProductIdWarehouseIdAndExpirationDateAsync(command.ProductId, command.WarehouseId, command.ExpirationDate)
            ?? throw new ArgumentException($"Inventory with product ID {command.ProductId} and warehouse ID {command.WarehouseId} does not exist.");
        
        // Decreases the stock of the product in the inventory.
        inventoryToUpdate.DecreaseStockFromProduct(command.QuantityToDecrease, product.MinimumStock.GetValue(), warehouse.AccountId);
        
        // Updates the inventory in the repository.
        await inventoryRepository.UpdateAsync(inventoryToUpdate.Id.ToString(), inventoryToUpdate);
        
        // Publishes the events related to the inventory.
        await inventoryRepository.PublishEventsAsync(inventoryToUpdate);
        
        // Returns the updated inventory.
        return inventoryToUpdate;
    }

    /// <summary>
    ///     Method to handle the removal of products from a warehouse without an expiration date.  
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for removing products from a warehouse.
    /// </param>
    /// <returns>
    ///     The inventory object representing the updated inventory or null if the inventory could not be updated.
    /// </returns>
    public async Task<Inventory?> Handle(DecreaseProductsFromWarehouseWithoutExpirationDateCommand command)
    {
        // Checks if the product exists.
        var product = await productRepository.FindByIdAsync(command.ProductId.ToString())
                      ?? throw new ArgumentException($"Product with ID {command.ProductId} does not exist.");
        
        // Checks if the warehouse exists.
        var warehouse = await warehouseRepository.FindByIdAsync(command.WarehouseId.ToString()) 
                        ?? throw new ArgumentException($"Warehouse with ID {command.WarehouseId} does not exist.");
        
        // Checks if the inventory exists.
        var inventoryToUpdate = await inventoryRepository.GetByProductIdWarehouseIdAsync(command.ProductId, command.WarehouseId)
                                ?? throw new ArgumentException($"Inventory with product ID {command.ProductId} and warehouse ID {command.WarehouseId} does not exist.");
        
        // Decreases the stock of the product in the inventory.
        inventoryToUpdate.DecreaseStockFromProduct(command.QuantityToDecrease, product.MinimumStock.GetValue(), warehouse.AccountId);
        
        // Updates the inventory in the repository.
        await inventoryRepository.UpdateAsync(inventoryToUpdate.Id.ToString(), inventoryToUpdate);
        
        // Publishes the events related to the inventory.
        await inventoryRepository.PublishEventsAsync(inventoryToUpdate);
        
        // Returns the updated inventory.
        return inventoryToUpdate;
    }

    /// <summary>
    ///     Method to handle the deletion of an inventory.
    ///     Use this when the stock is zero and is no longer needed. 
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for deleting an inventory.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation.
    /// </returns>
    public async Task Handle(DeleteInventoryCommand command)
    {
        // Verifies that the inventory exists.
        var inventoryToDelete = await inventoryRepository.FindByIdAsync(command.InventoryId.ToString())
                                ?? throw new ArgumentException($"Inventory with ID {command.InventoryId} does not exist.");
        
        // Deletes the inventory from the repository.
        await inventoryRepository.DeleteAsync(inventoryToDelete.Id.ToString());
    }
}
