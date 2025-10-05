using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Controllers;

/// <summary>
///     Controller for handling warehouse-related requests for a specific account.
/// </summary>
/// <param name="warehouseQueryService">
///     The service for handling warehouse-related queries.
/// </param>
[ApiController]
[Route("api/v1/accounts/{accountId}/warehouses")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Accounts")]
public class AccountWarehousesController(
        IWarehouseQueryService warehouseQueryService
    ) : ControllerBase
{
    /// <summary>
    ///     Endpoint to handle the retrieval of all warehouses for a specific account. 
    /// </summary>
    /// <param name="accountId">
    ///     The route parameter representing the unique identifier of the account for which to retrieve warehouses.
    /// </param>
    /// <returns>
    ///     A list of warehouses for the specified account, or a 404 Not Found response if no warehouses are found for the given account ID.
    /// </returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get All Warehouses by Account ID",
        Description = "Retrieves a list of warehouses by a specific Account ID.",
        OperationId = "GetAllWarehousesByAccountId")]
    [SwaggerResponse(StatusCodes.Status200OK, "Warehouses found!", typeof(List<WarehouseResource>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Warehouses not found for the given Account ID...")]
    public async Task<IActionResult> GetAllWarehousesByAccountId([FromRoute] string accountId)
    {
        var targetAccountId = new AccountId(accountId);
        var getAllWarehousesByAccountIdQuery = new GetAllWarehousesByAccountId(targetAccountId);
        var warehouses = await warehouseQueryService.Handle(getAllWarehousesByAccountIdQuery);
        if (warehouses.Count == 0) return NotFound($"No warehouses found for account ID {accountId}.");
        var warehouseList = warehouses.ToList();
        var resources = warehouseList
            .Select(WarehouseResourceFromEntityAssembler.ToResourceFromEntity)
            .ToList();
        return Ok(resources);
    }
}