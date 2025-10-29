using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;
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
public class AccountPurchaseOrderController(
    IPurchaseOrderCommandService purchaseOrderCommandService,
    IPurchaseOrderQueryService purchaseOrderQueryService) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new purchase order for an account.",
        Description = "Creates a new purchase order for the specified account.",
        OperationId = "CreatePurchaseOrderForAccount")]
    [SwaggerResponse(StatusCodes.Status201Created, "Purchase order created successfully.", typeof(PurchaseOrderResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request or failed creation.")]
    public async Task<IActionResult> CreatePurchaseOrder(string accountId, [FromBody] CreatePurchaseOrderForAccountResource resource)
    {
        try
        {
            var command = new CreatePurchaseOrderCommand(resource.orderCode, resource.catalogIdBuyFrom, accountId);
            var orderId = await purchaseOrderCommandService.Handle(command);

            var query = new GetPurchaseOrderByIdQuery(orderId.GetId);
            var order = await purchaseOrderQueryService.Handle(query);

            if (order == null)
                return BadRequest("Failed to create purchase order");

            var orderResource = PurchaseOrderResourceFromEntityAssembler.ToResourceFromEntity(order);
            return StatusCode(StatusCodes.Status201Created, orderResource);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

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