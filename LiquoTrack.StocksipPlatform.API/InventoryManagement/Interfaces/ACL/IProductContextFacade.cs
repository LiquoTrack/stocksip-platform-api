using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.ACL;

/// <summary>
/// ACL interface for exposing Product context operations to other bounded contexts.
/// </summary>
public interface IProductContextFacade
{
    /// <summary>
    /// Retrieves essential product information by its ID for use in other contexts.
    /// </summary>
    /// <param name="productId">The ID of the product to fetch.</param>
    /// <returns>A resource containing product information, or null if not found.</returns>
    Task<ProductDetailsResource?> GetProductDetailsByIdAsync(string productId);
}