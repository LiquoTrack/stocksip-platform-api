using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Services;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Controllers;

/// <summary>
/// Controller responsible for managing catalog operations.
/// </summary>
[ApiController]
[Route("api/v1/catalogs")]
[Produces(MediaTypeNames.Application.Json)]
public class CatalogController(
    ICatalogCommandService catalogCommandService,
    ICatalogQueryService catalogQueryService) : ControllerBase
{
    /// <summary>
    /// Creates a new catalog.
    /// </summary>
    /// <param name="resource">The resource containing catalog details.</param>
    /// <returns>The created catalog resource.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CatalogResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCatalog([FromBody] CreateCatalogResource resource)
    {
        try
        {
            var command = CreateCatalogCommandFromResourceAssembler.ToCommandFromResource(resource);
            var catalogId = await catalogCommandService.Handle(command);

            var query = new GetCatalogByIdQuery(catalogId.GetId());
            var catalog = await catalogQueryService.Handle(query);

            if (catalog == null)
                return BadRequest("Failed to create catalog.");

            var catalogResource = CatalogResourceFromEntityAssembler.ToResourceFromEntity(catalog);
            return CreatedAtAction(nameof(GetCatalogById), new { id = catalogId.GetId() }, catalogResource);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves a catalog by its identifier.
    /// </summary>
    /// <param name="id">The catalog identifier.</param>
    /// <returns>The catalog resource.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CatalogResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCatalogById(string id)
    {
        try
        {
            var query = new GetCatalogByIdQuery(id);
            var catalog = await catalogQueryService.Handle(query);

            if (catalog == null)
                return NotFound(new { message = $"Catalog with ID {id} not found." });

            var resource = CatalogResourceFromEntityAssembler.ToResourceFromEntity(catalog);
            return Ok(resource);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves all catalogs.
    /// </summary>
    /// <returns>A collection of catalog resources.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CatalogResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCatalogs()
    {
        var query = new GetAllCatalogsQuery();
        var catalogs = await catalogQueryService.Handle(query);
        var resources = CatalogResourceFromEntityAssembler.ToResourceFromEntity(catalogs);
        return Ok(resources);
    }

    /// <summary>
    /// Retrieves all published catalogs.
    /// </summary>
    /// <returns>A collection of published catalog resources.</returns>
    [HttpGet("published")]
    [ProducesResponseType(typeof(IEnumerable<CatalogResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPublishedCatalogs()
    {
        var query = new GetPublishedCatalogsQuery();
        var catalogs = await catalogQueryService.Handle(query);
        var resources = CatalogResourceFromEntityAssembler.ToResourceFromEntity(catalogs);
        return Ok(resources);
    }

    /// <summary>
    /// Updates a catalog by its identifier.
    /// </summary>
    /// <param name="id">The catalog identifier.</param>
    /// <param name="resource">The resource containing updated catalog data.</param>
    /// <returns>No content if successful.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCatalog(string id, [FromBody] UpdateCatalogResource resource)
    {
        try
        {
            var command = new UpdateCatalogCommand(id, resource.name, resource.description, resource.contactEmail);
            await catalogCommandService.Handle(command);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Publishes a catalog.
    /// </summary>
    /// <param name="id">The catalog identifier.</param>
    /// <returns>No content if successful.</returns>
    [HttpPut("{id}/publications")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PublishCatalog(string id)
    {
        try
        {
            var command = new PublishCatalogCommand(id);
            await catalogCommandService.Handle(command);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Unpublishes a catalog.
    /// </summary>
    /// <param name="id">The catalog identifier.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{id}/publications")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnpublishCatalog(string id)
    {
        try
        {
            var command = new UnpublishCatalogCommand(id);
            await catalogCommandService.Handle(command);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Adds an item to a catalog and returns the updated catalog as JSON.
    /// </summary>
    /// <param name="id">The catalog identifier.</param>
    /// <param name="resource">The resource containing item details (productId, amount, currency).</param>
    /// <returns>The updated catalog resource.</returns>
    [HttpPost("{id}/items")]
    [ProducesResponseType(typeof(CatalogResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddItemToCatalog(string id, [FromBody] AddItemToCatalogResource resource)
    {
        try
        {
            // Execute the add item command
            var command = new AddItemToCatalogCommand(id, resource.productId);
            await catalogCommandService.Handle(command);

            // Retrieve the updated catalog
            var query = new GetCatalogByIdQuery(id);
            var updatedCatalog = await catalogQueryService.Handle(query);

            if (updatedCatalog == null)
                return NotFound(new { message = $"Catalog with ID {id} not found." });

            var catalogResource = CatalogResourceFromEntityAssembler.ToResourceFromEntity(updatedCatalog);
            return Ok(catalogResource);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Removes an item from a catalog.
    /// </summary>
    /// <param name="id">The catalog identifier.</param>
    /// <param name="productId">The product identifier to remove.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{id}/items/{productId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveItemFromCatalog(string id, ProductId productId)
    {
        try
        {
            var command = new RemoveItemFromCatalogCommand(id, productId);
            await catalogCommandService.Handle(command);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}