using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Services;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Controllers;

/// <summary>
/// Controller responsible for managing catalog operations.
/// </summary>
[ApiController]
[Route("api/v1/catalogs")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available endpoints for managing catalogs.")]
public class CatalogController(
    ICatalogCommandService catalogCommandService,
    ICatalogQueryService catalogQueryService) : ControllerBase
{
    [HttpGet("{catalogId}")]
    [SwaggerOperation(
        Summary = "Get catalog by ID.",
        Description = "Retrieves a catalog by its unique identifier.",
        OperationId = "GetCatalogById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Catalog returned successfully.", typeof(CatalogResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Catalog not found.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
    public async Task<IActionResult> GetCatalogById(string catalogId)
    {
        try
        {
            var query = new GetCatalogByIdQuery(catalogId);
            var catalog = await catalogQueryService.Handle(query);

            if (catalog == null)
                return NotFound(new { message = $"Catalog with ID {catalogId} not found." });

            var resource = CatalogResourceFromEntityAssembler.ToResourceFromEntity(catalog);
            return Ok(resource);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all catalogs.",
        Description = "Retrieves all catalogs in the system.",
        OperationId = "GetAllCatalogs")]
    [SwaggerResponse(StatusCodes.Status200OK, "Catalogs retrieved successfully.", typeof(IEnumerable<CatalogResource>))]
    public async Task<IActionResult> GetAllCatalogs()
    {
        var query = new GetAllCatalogsQuery();
        var catalogs = await catalogQueryService.Handle(query);
        var resources = CatalogResourceFromEntityAssembler.ToResourceFromEntity(catalogs);
        return Ok(resources);
    }

    [HttpGet("published")]
    [SwaggerOperation(
        Summary = "Get all published catalogs.",
        Description = "Retrieves all catalogs that are published and visible.",
        OperationId = "GetPublishedCatalogs")]
    [SwaggerResponse(StatusCodes.Status200OK, "Published catalogs retrieved successfully.", typeof(IEnumerable<CatalogResource>))]
    public async Task<IActionResult> GetPublishedCatalogs()
    {
        var query = new GetPublishedCatalogsQuery();
        var catalogs = await catalogQueryService.Handle(query);
        var resources = CatalogResourceFromEntityAssembler.ToResourceFromEntity(catalogs);
        return Ok(resources);
    }

    [HttpPut("{catalogId}")]
    [SwaggerOperation(
        Summary = "Update a catalog.",
        Description = "Updates the details of an existing catalog by ID.",
        OperationId = "UpdateCatalog")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Catalog updated successfully.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Catalog not found.")]
    public async Task<IActionResult> UpdateCatalog(string catalogId, [FromBody] UpdateCatalogResource resource)
    {
        try
        {
            var command = new UpdateCatalogCommand(catalogId, resource.name, resource.description, resource.contactEmail);
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

    [HttpPut("{catalogId}/publications")]
    [SwaggerOperation(
        Summary = "Publish a catalog.",
        Description = "Marks a catalog as published and visible.",
        OperationId = "PublishCatalog")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Catalog published successfully.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Catalog not found.")]
    public async Task<IActionResult> PublishCatalog(string catalogId)
    {
        try
        {
            var command = new PublishCatalogCommand(catalogId);
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

    [HttpDelete("{catalogId}/publications")]
    [SwaggerOperation(
        Summary = "Unpublish a catalog.",
        Description = "Marks a catalog as unpublished and hidden.",
        OperationId = "UnpublishCatalog")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Catalog unpublished successfully.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Catalog not found.")]
    public async Task<IActionResult> UnpublishCatalog(string catalogId)
    {
        try
        {
            var command = new UnpublishCatalogCommand(catalogId);
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

    [HttpPost("{catalogId}/items")]
    [SwaggerOperation(
        Summary = "Add item to catalog.",
        Description = "Adds a product item to an existing catalog and returns the updated catalog.",
        OperationId = "AddItemToCatalog")]
    [SwaggerResponse(StatusCodes.Status200OK, "Item added and catalog updated successfully.", typeof(CatalogResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Catalog not found.")]
    public async Task<IActionResult> AddItemToCatalog(string catalogId, [FromBody] AddItemToCatalogResource resource)
    {
        try
        {
            var command = new AddItemToCatalogCommand(catalogId, resource.productId);
            await catalogCommandService.Handle(command);

            var query = new GetCatalogByIdQuery(catalogId);
            var updatedCatalog = await catalogQueryService.Handle(query);

            if (updatedCatalog == null)
                return NotFound(new { message = $"Catalog with ID {catalogId} not found." });

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

    [HttpDelete("{catalogId}/items/{productId}")]
    [SwaggerOperation(
        Summary = "Remove item from catalog.",
        Description = "Removes a product item from a catalog.",
        OperationId = "RemoveItemFromCatalog")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Item removed successfully.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Catalog or item not found.")]
    public async Task<IActionResult> RemoveItemFromCatalog(string catalogId, ProductId productId)
    {
        try
        {
            var command = new RemoveItemFromCatalogCommand(catalogId, productId);
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