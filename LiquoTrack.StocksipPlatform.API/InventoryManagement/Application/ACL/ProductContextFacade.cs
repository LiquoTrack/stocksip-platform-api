using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.ACL;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;
using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.ACL;

/// <summary>
/// Facade for handling product-related operations, exposed to other bounded contexts.
/// </summary>
public class ProductContextFacade : IProductContextFacade
{
    private readonly IProductQueryService _productQueryService;
    private readonly ILogger<ProductContextFacade> _logger;

    public ProductContextFacade(
        IProductQueryService productQueryService,
        ILogger<ProductContextFacade> logger)
    {
        _productQueryService = productQueryService ?? throw new ArgumentNullException(nameof(productQueryService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ProductDetailsResource?> GetProductDetailsByIdAsync(string productId)
    {
        if (!ObjectId.TryParse(productId, out var objectId))
            throw new ArgumentException("Invalid product ID format.", nameof(productId));

        try
        {
            _logger.LogInformation("Fetching product details for product ID: {ProductId}", productId);

            var query = new Domain.Model.Queries.GetProductByIdQuery(objectId);
            var product = await _productQueryService.Handle(query);

            if (product == null)
            {
                _logger.LogWarning("No product found with ID: {ProductId}", productId);
                return null;
            }

            var dto = new ProductDetailsResource
            {
                Id = product.Id.ToString(),
                Name = product.Name,
                Brand = product.Brand,
                Type = product.Type.ToString(),
                UnitPrice = product.UnitPrice.GetAmount(),
                Currency = product.UnitPrice.GetCurrencyCode(),
                ImageUrl = product.ImageUrl.GetValue()
            };

            _logger.LogInformation("Successfully retrieved product details for ID: {ProductId}", productId);
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product details for ID: {ProductId}", productId);
            throw new InvalidOperationException("An error occurred while fetching product details.", ex);
        }
    }
}