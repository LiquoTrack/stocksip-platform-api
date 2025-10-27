using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Services;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Controllers;

/// <summary>
/// Controller for account-specific catalog operations.
/// </summary>
[ApiController]
[Route("api/v1/accounts/{accountId}/catalogs")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Accounts")]
public class AccountCatalogController(ICatalogQueryService catalogQueryService) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all catalogs by account.",
        Description = "Retrieves all catalogs owned by the specified account.",
        OperationId = "GetCatalogsByAccount")]
    [SwaggerResponse(StatusCodes.Status200OK, "Catalogs retrieved successfully.", typeof(IEnumerable<CatalogResource>))]
    public async Task<IActionResult> GetCatalogsByAccount(string accountId)
    {
        var query = new GetCatalogsByOwnerQuery(accountId);
        var catalogs = await catalogQueryService.Handle(query);
        var resources = CatalogResourceFromEntityAssembler.ToResourceFromEntity(catalogs);
        return Ok(resources);
    }
}