using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Services;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Controllers;

/// <summary>
/// Controller for purchase order operations.
/// </summary>
[ApiController]
[Route("api/v1/purchase-orders")]
[Produces(MediaTypeNames.Application.Json)]
public class PurchaseOrderController(
    IPurchaseOrderCommandService purchaseOrderCommandService,
    IPurchaseOrderQueryService purchaseOrderQueryService) : ControllerBase
{
    /// <summary>
    /// Creates a new purchase order.
    /// </summary>
    /// <param name="resource">The resource containing purchase order details.</param>
    /// <returns>The created purchase order resource.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(PurchaseOrderResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Gets a purchase order by its ID.
    /// </summary>
    /// <param name="id">The purchase order identifier.</param>
    /// <returns>The purchase order resource.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PurchaseOrderResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Gets all purchase orders.
    /// </summary>
    /// <returns>A collection of purchase order resources.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PurchaseOrderResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPurchaseOrders()
    {
        var query = new GetAllPurchaseOrdersQuery();
        var orders = await purchaseOrderQueryService.Handle(query);
        var resources = PurchaseOrderResourceFromEntityAssembler.ToResourceFromEntity(orders);
        return Ok(resources);
    }

    /// <summary>
    /// Adds an item to a purchase order.
    /// </summary>
    /// <param name="id">The purchase order identifier.</param>
    /// <param name="resource">The resource containing item details.</param>
    /// <returns>No content on success.</returns>
    [HttpPost("{id}/items")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddItemToOrder(string id, [FromBody] AddItemToOrderResource resource)
    {
        try
        {
            var command = new AddItemToOrderCommand(id, resource.productId, resource.unitPrice, resource.amountToPurchase);
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

    /// <summary>
    /// Removes an item from a purchase order.
    /// </summary>
    /// <param name="id">The purchase order identifier.</param>
    /// <param name="productId">The product identifier to remove.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id}/items/{productId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Confirms a purchase order.
    /// </summary>
    /// <param name="id">The purchase order identifier.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("{id}/confirmations")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Ships a purchase order.
    /// </summary>
    /// <param name="id">The purchase order identifier.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("{id}/shipments")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Receives a purchase order.
    /// </summary>
    /// <param name="id">The purchase order identifier.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("{id}/receptions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Cancels a purchase order.
    /// </summary>
    /// <param name="id">The purchase order identifier.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("{id}/cancellations")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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