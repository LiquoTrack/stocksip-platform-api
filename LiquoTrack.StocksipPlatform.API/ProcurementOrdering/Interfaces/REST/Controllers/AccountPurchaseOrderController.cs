using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Services;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all purchase orders by account.",
        Description = "Retrieves all purchase orders for the specified account (buyer).",
        OperationId = "GetPurchaseOrdersByAccount")]
    [SwaggerResponse(StatusCodes.Status200OK, "Purchase orders retrieved successfully.", typeof(IEnumerable<PurchaseOrderResource>))]
    public async Task<IActionResult> GetPurchaseOrdersByAccount(string accountId)
    {
        var query = new GetOrdersByBuyerQuery(accountId);
        var orders = await purchaseOrderQueryService.Handle(query);
        var resources = PurchaseOrderResourceFromEntityAssembler.ToResourceFromEntity(orders);
        return Ok(resources);
    }
}