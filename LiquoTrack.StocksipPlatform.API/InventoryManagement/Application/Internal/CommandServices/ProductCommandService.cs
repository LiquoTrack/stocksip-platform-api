using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Exceptions;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.CommandServices;

public class ProductCommandService(
        IProductRepository productRepository
    ) : IProductCommandService
{
    /// <summary>
    ///     Method to handle the registration of a new product.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for registering a new product.
    /// </param>
    /// <returns>
    ///     The newly registered product.
    ///     Or null if the product could not be registered.
    /// </returns>
    public async Task<Product?> Handle(RegisterProductCommand command)
    {
        // Verifies that the product name is unique.
        if (await productRepository.ExistsByNameAsync(new ProductName(command.Name)))
        {
            throw new ProductFailedCreationException("This ${command.Name} is taken by another product. Cannot create a new product with the same name.");
        }

        // Creates the product with the given details.
        var product = new Product(command);

        // Tries to add the product to the repository.
        try
        {
            await productRepository.AddAsync(product);
            return product;
        }
        // If the product could not be added, throws an exception.
        catch (ProductFailedCreationException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    ///     Method to handle the update of product information.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for updating the product information.
    /// </param>
    /// <returns>
    ///     The updated product.
    ///     Or null if the product could not be updated.
    /// </returns>
    public async Task<Product?> Handle(UpdateProductInformationCommand command)
    {
        // Verifies that the product exists.
        var productToUpdate = await productRepository.FindByIdAsync(command.ProductId.ToString())
                              ?? throw new ProductFailedUpdateException($"Could not find the product to update with identifier ${command.ProductId.ToString()}.");
        
        // Updates the product with the given details.
        productToUpdate.UpdateInformation(command);

        // Tries to update the product in the repository.
        try
        {
            await productRepository.UpdateAsync(command.ProductId.ToString(), productToUpdate);
        }
        // If the product could not be updated, throws an exception.
        catch (ProductFailedUpdateException e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        // Returns the updated product.
        return productToUpdate;
    }

    /// <summary>
    ///     Method to handle the update of the minimum stock related to a product.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for updating the minimum stock.
    /// </param>
    /// <returns>
    ///     The updated product.
    ///     Or null if the product could not be updated.   
    /// </returns>
    public async Task<Product?> Handle(UpdateProductMinimumStockCommand command)
    {
        // Verifies that the product exists.
        var productToUpdate = await productRepository.FindByIdAsync(command.ProductId.ToString())
                              ?? throw new ProductFailedUpdateException($"Could not find the product to update with identifier ${command.ProductId.ToString()}.");
        
        // Updates the product with the given details.
        productToUpdate.UpdateMinimumStock(command);

        // Tries to update the product minimum stock in the repository.
        try
        {
            await productRepository.UpdateAsync(command.ProductId.ToString(), productToUpdate);
        }
        // If the product could not be updated, throws an exception.
        catch (ProductFailedUpdateException e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        // Returns the updated product.
        return productToUpdate;
    }

    /// <summary>
    ///     Method to delete a product.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for deleting a product.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation.
    ///     No return value.
    /// </returns>
    public async Task Handle(DeleteProductCommand command)
    {
        // Verifies that the product exists.
        var productToDelete = await productRepository.FindByIdAsync(command.ProductId.ToString())
                              ?? throw new ProductFailedDeletionException($"Could not find the product to delete with identifier ${command.ProductId.ToString()}.");

        // Tries to delete the product from the repository.
        try
        {
            await productRepository.DeleteAsync(command.ProductId.ToString());
        }
        // If the product could not be deleted, throws an exception.
        catch (ProductFailedDeletionException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}