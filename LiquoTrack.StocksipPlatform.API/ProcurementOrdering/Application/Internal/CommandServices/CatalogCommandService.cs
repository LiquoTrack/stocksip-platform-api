using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Application.Internal.CommandServices;

/// <summary>
/// Service implementation for handling catalog commands.
/// </summary>
public class CatalogCommandService(ICatalogRepository catalogRepository) : ICatalogCommandService
{
    /// <summary>
    /// Handles the CreateCatalogCommand to create a new catalog.
    /// </summary>
    /// <param name="command">The command containing the catalog details.</param>
    /// <returns>The identifier of the created catalog.</returns>
    public async Task<CatalogId> Handle(CreateCatalogCommand command)
    {
        var catalog = new Catalog(command);
        await catalogRepository.CreateAsync(catalog);
        return catalog.Id;
    }

    /// <summary>
    /// Handles the UpdateCatalogCommand to update an existing catalog.
    /// </summary>
    /// <param name="command">The command containing the updated catalog information.</param>
    /// <exception cref="InvalidOperationException">Thrown when the catalog is not found.</exception>
    public async Task Handle(UpdateCatalogCommand command)
    {
        var catalogId = new CatalogId(command.catalogId);
        var catalog = await catalogRepository.GetByIdAsync(catalogId);

        if (catalog == null)
            throw new InvalidOperationException($"Catalog with ID {command.catalogId} not found");

        catalog.UpdateCatalog(command);
        await catalogRepository.UpdateAsync(catalog);
    }

    /// <summary>
    /// Handles the PublishCatalogCommand to publish a catalog.
    /// </summary>
    /// <param name="command">The command containing the catalog identifier.</param>
    /// <exception cref="InvalidOperationException">Thrown when the catalog is not found.</exception>
    public async Task Handle(PublishCatalogCommand command)
    {
        var catalogId = new CatalogId(command.catalogId);
        var catalog = await catalogRepository.GetByIdAsync(catalogId);

        if (catalog == null)
            throw new InvalidOperationException($"Catalog with ID {command.catalogId} not found");

        catalog.PublishCatalog();
        await catalogRepository.UpdateAsync(catalog);
    }

    /// <summary>
    /// Handles the UnpublishCatalogCommand to unpublish a catalog.
    /// </summary>
    /// <param name="command">The command containing the catalog identifier.</param>
    /// <exception cref="InvalidOperationException">Thrown when the catalog is not found.</exception>
    public async Task Handle(UnpublishCatalogCommand command)
    {
        var catalogId = new CatalogId(command.catalogId);
        var catalog = await catalogRepository.GetByIdAsync(catalogId);

        if (catalog == null)
            throw new InvalidOperationException($"Catalog with ID {command.catalogId} not found");

        catalog.UnpublishCatalog();
        await catalogRepository.UpdateAsync(catalog);
    }

    /// <summary>
    /// Handles the AddItemToCatalogCommand to add an item to a catalog.
    /// </summary>
    /// <param name="command">The command containing the item details.</param>
    /// <exception cref="InvalidOperationException">Thrown when the catalog is not found.</exception>
    public async Task Handle(AddItemToCatalogCommand command)
    {
        var catalogId = new CatalogId(command.catalogId);
        var catalog = await catalogRepository.GetByIdAsync(catalogId);

        if (catalog == null)
            throw new InvalidOperationException($"Catalog with ID {command.catalogId} not found");

        catalog.AddItem(command);
        await catalogRepository.UpdateAsync(catalog);
    }

    /// <summary>
    /// Handles the RemoveItemFromCatalogCommand to remove an item from a catalog.
    /// </summary>
    /// <param name="command">The command containing the product identifier.</param>
    /// <exception cref="InvalidOperationException">Thrown when the catalog is not found.</exception>
    public async Task Handle(RemoveItemFromCatalogCommand command)
    {
        var catalogId = new CatalogId(command.catalogId);
        var catalog = await catalogRepository.GetByIdAsync(catalogId);

        if (catalog == null)
            throw new InvalidOperationException($"Catalog with ID {command.catalogId} not found");

        catalog.RemoveItem(command);
        await catalogRepository.UpdateAsync(catalog);
    }
}