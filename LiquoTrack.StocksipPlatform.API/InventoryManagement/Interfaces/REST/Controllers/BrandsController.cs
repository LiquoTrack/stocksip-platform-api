using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Controllers;

/// <summary>
///     Controller for handling brand-related requests.
/// </summary>
/// <param name="brandQueryService">
///     The service for handling brand-related queries.
/// </param>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available endpoints for brands.")]
public class BrandsController(
        IBrandQueryService brandQueryService
    ) : ControllerBase
{
    /// <summary>
    ///     Endpoint to handle the retrieval of all brands.   
    /// </summary>
    /// <returns>
    ///     The list of all brands. 
    /// </returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all brands.",
        Description = "Retrieves a list of all brand names.",
        OperationId = "GetAllBrands")]
    [SwaggerResponse(StatusCodes.Status200OK, "Brands retrieved successfully.", typeof(IEnumerable<BrandResource>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Brands could not be retrieved.")]
    public async Task<IActionResult> GetAllBrands()
    {
        var getAllBrandsQuery = new GetAllBrandsQuery();
        var brands = await brandQueryService.Handle(getAllBrandsQuery);
        var enumerable = brands as Brand[] ?? brands.ToArray();
        if (enumerable.Length == 0) return NotFound("No brands found.");
        var brandResources = enumerable.Select(BrandResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(brandResources);
    }
}