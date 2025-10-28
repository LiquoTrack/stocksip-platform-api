using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Services;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;

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
    /// <summary>
    /// Gets all catalogs owned by a specific account.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <returns>A collection of catalog resources.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CatalogResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCatalogsByAccount(string accountId)
    {
        var query = new GetCatalogsByOwnerQuery(accountId);
        var catalogs = await catalogQueryService.Handle(query);
        var resources = CatalogResourceFromEntityAssembler.ToResourceFromEntity(catalogs);
        return Ok(resources);
    }
}