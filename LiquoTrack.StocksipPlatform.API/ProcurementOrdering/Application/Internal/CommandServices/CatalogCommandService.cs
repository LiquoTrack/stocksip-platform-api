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

    /// <summary>
    /// Handles the CreateCatalogCommand.
    /// </summary>
    public async Task<CatalogId> Handle(CreateCatalogCommand command)
    {
        var catalog = new Catalog(command);
        await _catalogRepository.AddAsync(catalog);
        return catalog.CatalogId;
    }

    /// <summary>
    /// Handles the UpdateCatalogCommand.
    /// </summary>
    public async Task Handle(UpdateCatalogCommand command)
    {
        var catalog = await GetCatalogByIdAsync(command.catalogId);
        catalog.UpdateCatalog(command);
        await _catalogRepository.UpdateAsync(catalog);
    }

    /// <summary>
    /// Handles the PublishCatalogCommand.
    /// </summary>
    public async Task Handle(PublishCatalogCommand command)
    {
        var catalog = await GetCatalogByIdAsync(command.catalogId);
        catalog.PublishCatalog();
        await _catalogRepository.UpdateAsync(catalog);
    }

    /// <summary>
    /// Handles the UnpublishCatalogCommand.
    /// </summary>
    public async Task Handle(UnpublishCatalogCommand command)
    {
        var catalog = await GetCatalogByIdAsync(command.catalogId);
        catalog.UnpublishCatalog();
        await _catalogRepository.UpdateAsync(catalog);
    }

    /// <summary>
    /// Handles the AddItemToCatalogCommand.
    /// Fetches product details and stock from the Inventory context and adds it to the catalog.
    /// </summary>
    public async Task Handle(AddItemToCatalogCommand command)
    {
        var catalog = await GetCatalogByIdAsync(command.CatalogId);

        // Fetch product details
        var product = await _productContextFacade.GetProductDetailsByIdAsync(command.ProductId)
            ?? throw new InvalidOperationException($"Product with ID {command.ProductId} not found");

        // Fetch stock
        var stock = await _productContextFacade.GetProductStockInWarehouseAsync(command.ProductId, command.WarehouseId)
            ?? throw new InvalidOperationException($"No inventory found for product {command.ProductId} in warehouse {command.WarehouseId}");

        // Add item with stock
        catalog.AddItem(product.Id, product.Name, product.UnitPrice, product.Currency, product.ImageUrl, stock);
        await _catalogRepository.UpdateAsync(catalog);
    }

    /// <summary>
    /// Handles the RemoveItemFromCatalogCommand.
    /// </summary>
    public async Task Handle(RemoveItemFromCatalogCommand command)
    {
        var catalog = await GetCatalogByIdAsync(command.catalogId);
        catalog.RemoveItem(command);
        await _catalogRepository.UpdateAsync(catalog);
    }

    /// <summary>
    /// Helper method to get a catalog by ID or throw an exception if not found.
    /// </summary>
    private async Task<Catalog> GetCatalogByIdAsync(string catalogId)
    {
        var id = new CatalogId(catalogId);
        return await _catalogRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"Catalog with ID {catalogId} not found");
    }
}