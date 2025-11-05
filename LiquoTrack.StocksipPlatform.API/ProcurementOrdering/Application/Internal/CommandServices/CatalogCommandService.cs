using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.ACL;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Application.Internal.CommandServices;

/// <summary>
/// Command service responsible for handling catalog-related operations.
/// </summary>
public class CatalogCommandService : ICatalogCommandService
{
    private readonly ICatalogRepository _catalogRepository;
    private readonly IProductContextFacade _productContextFacade;

    public CatalogCommandService(
        ICatalogRepository catalogRepository,
        IProductContextFacade productContextFacade)
    {
        _catalogRepository = catalogRepository ?? throw new ArgumentNullException(nameof(catalogRepository));
        _productContextFacade = productContextFacade ?? throw new ArgumentNullException(nameof(productContextFacade));
    }

    public async Task<CatalogId> Handle(CreateCatalogCommand command)
    {
        var catalog = new Catalog(command);
        await _catalogRepository.AddAsync(catalog);
        return catalog.CatalogId;
    }

    public async Task Handle(UpdateCatalogCommand command)
    {
        var catalogId = new CatalogId(command.catalogId);
        var catalog = await _catalogRepository.GetByIdAsync(catalogId)
                      ?? throw new InvalidOperationException($"Catalog with ID {command.catalogId} not found");

        catalog.UpdateCatalog(command);
        await _catalogRepository.UpdateAsync(catalog);
    }

    public async Task Handle(PublishCatalogCommand command)
    {
        var catalogId = new CatalogId(command.catalogId);
        var catalog = await _catalogRepository.GetByIdAsync(catalogId)
                      ?? throw new InvalidOperationException($"Catalog with ID {command.catalogId} not found");

        catalog.PublishCatalog();
        await _catalogRepository.UpdateAsync(catalog);
    }

    public async Task Handle(UnpublishCatalogCommand command)
    {
        var catalogId = new CatalogId(command.catalogId);
        var catalog = await _catalogRepository.GetByIdAsync(catalogId)
                      ?? throw new InvalidOperationException($"Catalog with ID {command.catalogId} not found");

        catalog.UnpublishCatalog();
        await _catalogRepository.UpdateAsync(catalog);
    }

    /// <summary>
    /// Adds a product as an item to a catalog, fetching product data from the Inventory context.
    /// </summary>
    public async Task Handle(AddItemToCatalogCommand command)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var catalogId = new CatalogId(command.CatalogId);
        var catalog = await _catalogRepository.GetByIdAsync(catalogId)
                      ?? throw new InvalidOperationException($"Catalog with ID {command.CatalogId} not found");

        // Fetch product details from the Inventory context
        var product = await _productContextFacade.GetProductDetailsByIdAsync(command.ProductId);
        if (product == null)
            throw new InvalidOperationException($"Product with ID {command.ProductId} not found in Inventory context");

        // Add item to catalog using product data
        catalog.AddItem(
            product.Id,
            product.Name,
            product.UnitPrice,
            product.Currency,
            product.ImageUrl
        );

        await _catalogRepository.UpdateAsync(catalog);
    }

    public async Task Handle(RemoveItemFromCatalogCommand command)
    {
        var catalogId = new CatalogId(command.catalogId);
        var catalog = await _catalogRepository.GetByIdAsync(catalogId)
                      ?? throw new InvalidOperationException($"Catalog with ID {command.catalogId} not found");

        catalog.RemoveItem(command);
        await _catalogRepository.UpdateAsync(catalog);
    }
}