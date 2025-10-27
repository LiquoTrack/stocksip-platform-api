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
/// Controller for purchase order operations.
/// </summary>
[ApiController]
[Route("api/v1/purchase-orders")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available endpoints for managing purchase orders.")]
public class PurchaseOrderController(
    IPurchaseOrderCommandService purchaseOrderCommandService,
    IPurchaseOrderQueryService purchaseOrderQueryService) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new purchase order.",
        Description = "Creates a new purchase order with the specified details.",
        OperationId = "CreatePurchaseOrder")]
    [SwaggerResponse(StatusCodes.Status201Created, "Purchase order created successfully.", typeof(PurchaseOrderResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request or failed creation.")]
    public async Task<IActionResult> CreatePurchaseOrder([FromBody] CreatePurchaseOrderResource resource)
    {
        try
        {
            var command = CreatePurchaseOrderCommandFromResourceAssembler.ToCommandFromResource(resource);
            var orderId = await purchaseOrderCommandService.Handle(command);

            var query = new GetPurchaseOrderByIdQuery(orderId.GetId);
            var order = await purchaseOrderQueryService.Handle(query);

            if (order == null)
                return BadRequest("Failed to create purchase order");

            var orderResource = PurchaseOrderResourceFromEntityAssembler.ToResourceFromEntity(order);
            return CreatedAtAction(nameof(GetPurchaseOrderById), new { id = orderId.GetId }, orderResource);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{purchaseOrderId}")]
    [SwaggerOperation(
        Summary = "Get purchase order by ID.",
        Description = "Retrieves a purchase order by its unique identifier.",
        OperationId = "GetPurchaseOrderById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Purchase order returned successfully.", typeof(PurchaseOrderResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Purchase order not found.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
    public async Task<IActionResult> GetPurchaseOrderById(string id)
    {
        try
        {
            var query = new GetPurchaseOrderByIdQuery(id);
            var order = await purchaseOrderQueryService.Handle(query);

            if (order == null)
                return NotFound(new { message = $"Purchase order with ID {id} not found" });

            var resource = PurchaseOrderResourceFromEntityAssembler.ToResourceFromEntity(order);
            return Ok(resource);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all purchase orders.",
        Description = "Retrieves all purchase orders in the system.",
        OperationId = "GetAllPurchaseOrders")]
    [SwaggerResponse(StatusCodes.Status200OK, "Purchase orders retrieved successfully.", typeof(IEnumerable<PurchaseOrderResource>))]
    public async Task<IActionResult> GetAllPurchaseOrders()
    {
        var query = new GetAllPurchaseOrdersQuery();
        var orders = await purchaseOrderQueryService.Handle(query);
        var resources = PurchaseOrderResourceFromEntityAssembler.ToResourceFromEntity(orders);
        return Ok(resources);
    }

    [HttpPost("{purchaseOrderId}/items")]
    [SwaggerOperation(
        Summary = "Add item to purchase order.",
        Description = "Adds a product item to an existing purchase order.",
        OperationId = "AddItemToPurchaseOrder")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Item added successfully.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Purchase order not found.")]
    public async Task<IActionResult> AddItemToOrder(string id, [FromBody] AddItemToOrderResource resource)
    {
        try
        {
            var quantity = resource.quantity ?? 1;
            var command = new AddItemToOrderCommand(id, resource.productId, quantity);
            await purchaseOrderCommandService.Handle(command);
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

    [HttpDelete("{purchaseOrderId}/items/{productId}")]
    [SwaggerOperation(
        Summary = "Remove item from purchase order.",
        Description = "Removes a product item from an existing purchase order.",
        OperationId = "RemoveItemFromPurchaseOrder")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Item removed successfully.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Purchase order or item not found.")]
    public async Task<IActionResult> RemoveItemFromOrder(string id, string productId)
    {
        try
        {
            var command = new RemoveItemFromOrderCommand(id, productId);
            await purchaseOrderCommandService.Handle(command);
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

    [HttpPut("{purchaseOrderId}/confirmations")]
    [SwaggerOperation(
        Summary = "Confirm purchase order.",
        Description = "Confirms an existing purchase order.",
        OperationId = "ConfirmPurchaseOrder")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Order confirmed successfully.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Purchase order not found or cannot be confirmed.")]
    public async Task<IActionResult> ConfirmOrder(string id)
    {
        try
        {
            var command = new ConfirmOrderCommand(id);
            await purchaseOrderCommandService.Handle(command);
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

    [HttpPut("{purchaseOrderId}/shipments")]
    [SwaggerOperation(
        Summary = "Ship purchase order.",
        Description = "Marks a purchase order as shipped.",
        OperationId = "ShipPurchaseOrder")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Order shipped successfully.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Purchase order not found or cannot be shipped.")]
    public async Task<IActionResult> ShipOrder(string id)
    {
        try
        {
            var command = new ShipOrderCommand(id);
            await purchaseOrderCommandService.Handle(command);
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

    [HttpPut("{purchaseOrderId}/receptions")]
    [SwaggerOperation(
        Summary = "Receive purchase order.",
        Description = "Marks a purchase order as received.",
        OperationId = "ReceivePurchaseOrder")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Order received successfully.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Purchase order not found or cannot be received.")]
    public async Task<IActionResult> ReceiveOrder(string id)
    {
        try
        {
            var command = new ReceiveOrderCommand(id);
            await purchaseOrderCommandService.Handle(command);
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

    [HttpPut("{purchaseOrderId}/cancellations")]
    [SwaggerOperation(
        Summary = "Cancel purchase order.",
        Description = "Cancels an existing purchase order.",
        OperationId = "CancelPurchaseOrder")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Order canceled successfully.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Purchase order not found or cannot be canceled.")]
    public async Task<IActionResult> CancelOrder(string id)
    {
        try
        {
            var command = new CancelOrderCommand(id);
            await purchaseOrderCommandService.Handle(command);
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