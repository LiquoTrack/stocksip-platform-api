using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Services;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Controllers;

/// <summary>
/// Controller for account-specific purchase order operations.
/// </summary>
[ApiController]
[Route("api/v1/accounts/{accountId}/purchase-orders")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Accounts")]
public class AccountPurchaseOrderController(IPurchaseOrderQueryService purchaseOrderQueryService) : ControllerBase
{
    /// <summary>
    /// Gets all purchase orders for a specific account (buyer).
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <returns>A collection of purchase order resources.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PurchaseOrderResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPurchaseOrdersByAccount(string accountId)
    {
        var query = new GetOrdersByBuyerQuery(accountId);
        var orders = await purchaseOrderQueryService.Handle(query);
        var resources = PurchaseOrderResourceFromEntityAssembler.ToResourceFromEntity(orders);
        return Ok(resources);
    }
}