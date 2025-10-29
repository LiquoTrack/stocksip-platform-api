using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Security.Claims;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using Microsoft.AspNetCore.Authorization;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Controllers
{
    [ApiController]
    [Route("api/v1/orders")]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerTag("Available Sales Order Endpoints")]
    public class SalesOrderController(
        ISalesOrderCommandService salesOrderCommandService,
        ISalesOrderQueryService salesOrderQueryService,
        ISalesOrderRepository salesOrderRepository,
        AppDbContext dbContext)
        : ControllerBase
    {
        [HttpPost("generate-purchase-order")]
        [Authorize(Roles = "LiquorStoreOwner")]
        [SwaggerOperation(Summary = "Generate a new purchase order")]
        [SwaggerResponse(StatusCodes.Status201Created, "Purchase order created successfully", typeof(SalesOrderResource))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request data")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have permission to generate purchase orders")]
        public async Task<IActionResult> GeneratePurchaseOrder([FromBody] GeneratePurchaseOrderRequest request)
        {
            try
            {
                var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                 ?? User.FindFirst("sid")?.Value
                                 ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid")?.Value;
                
                if (string.IsNullOrEmpty(accountId))
                {
                    return Unauthorized("User not authenticated");
                }
                var order = await salesOrderCommandService.GeneratePurchaseOrder(request, accountId);
                var resource = SalesOrderResourceFromEntityAssembler.ToResourceFromEntity(order);
                
                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, resource);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get order by ID")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns the order", typeof(SalesOrderResource))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found")]
        public async Task<IActionResult> GetOrderById(string id)
        {
            var order = await salesOrderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            
            var resource = SalesOrderResourceFromEntityAssembler.ToResourceFromEntity(order);
            return Ok(resource);
        }
        
        [HttpGet]
        [SwaggerOperation(Summary = "Get all orders")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns all orders", typeof(IEnumerable<SalesOrderResource>))]
        [SwaggerResponse(StatusCodes.Status200OK, "No orders found", Type = typeof(IEnumerable<SalesOrderResource>))]
        public async Task<IActionResult> GetAllOrders()
        {
            var query = new GetAllSalesOrdersQuery();
            var orders = await salesOrderQueryService.Handle(query);
            
            var resources = orders.Select(SalesOrderResourceFromEntityAssembler.ToResourceFromEntity);
            return Ok(resources);
        }

        [HttpPost("{orderId}/delivery/propose")]
        [Authorize(Roles = "Supplier")]
        [SwaggerOperation(Summary = "Propose a delivery schedule for an order (Supplier)")]
        [SwaggerResponse(StatusCodes.Status200OK, "Delivery proposal saved", typeof(SalesOrderResource))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found")]
        public async Task<IActionResult> ProposeDeliverySchedule([FromRoute] string orderId, [FromBody] ProposeDeliveryScheduleRequest request)
        {
            try
            {
                var command = new ProposeDeliveryScheduleCommand(orderId, request.ProposedDate, request.Notes);
                var updated = await salesOrderCommandService.Handle(command);
                var resource = SalesOrderResourceFromEntityAssembler.ToResourceFromEntity(updated);
                return Ok(resource);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("{orderId}/delivery/propose")]
        [Authorize(Roles = "Supplier")]
        [SwaggerOperation(Summary = "Get current delivery proposal for an order (Supplier)")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns the order with current delivery proposal", typeof(SalesOrderResource))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found")]
        public async Task<IActionResult> GetProposedDelivery([FromRoute] string orderId)
        {
            var order = await salesOrderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return NotFound(new { message = $"Order with ID {orderId} not found" });
            }

            var resource = SalesOrderResourceFromEntityAssembler.ToResourceFromEntity(order);
            return Ok(resource);
        }

        [HttpPost("{orderId}/delivery/respond")]
        [Authorize(Roles = "LiquorStoreOwner")]
        [SwaggerOperation(Summary = "Respond to a delivery proposal (LiquorStoreOwner)")]
        [SwaggerResponse(StatusCodes.Status200OK, "Delivery proposal response saved", typeof(SalesOrderResource))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found")]
        public async Task<IActionResult> RespondDeliveryProposal([FromRoute] string orderId, [FromBody] RespondDeliveryProposalRequest request)
        {
            try
            {
                var command = new RespondDeliveryProposalCommand(orderId, request.Accept, request.Notes);
                var updated = await salesOrderCommandService.Handle(command);
                var resource = SalesOrderResourceFromEntityAssembler.ToResourceFromEntity(updated);
                return Ok(resource);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("{orderId}/delivery/respond")]
        [Authorize(Roles = "LiquorStoreOwner")]
        [SwaggerOperation(Summary = "Get current delivery proposal and response (LiquorStoreOwner)")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns the order with current delivery proposal", typeof(SalesOrderResource))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found")]
        public async Task<IActionResult> GetDeliveryProposalForResponse([FromRoute] string orderId)
        {
            var order = await salesOrderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return NotFound(new { message = $"Order with ID {orderId} not found" });
            }

            var resource = SalesOrderResourceFromEntityAssembler.ToResourceFromEntity(order);
            return Ok(resource);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create order")]
        [SwaggerResponse(StatusCodes.Status201Created, "Order created", typeof(CreateSalesOrderResource))]
        public async Task<IActionResult> CreateOrder([FromBody] CreateSalesOrderResource body)
        {
            // Get the account ID from the JWT token claims
            var accountId = User.FindFirst("sid")?.Value
                             ?? User.FindFirst(ClaimTypes.Sid)?.Value
                             ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid")?.Value
                             ?? throw new UnauthorizedAccessException("Account ID not found in token");

            // Log the account ID for debugging
            Console.WriteLine($"Account ID from token: {accountId}");

            var cmd = CreateSalesOrderCommandFromResourceAssembler.ToCommandFromResource(body, accountId);
            var created = await salesOrderCommandService.Handle(cmd);

            var resource = SalesOrderResourceFromEntityAssembler.ToResourceFromEntity(created);

            return Created($"/api/v1/orders/{created.Id}", resource);
        }
        
        [HttpPut("{orderId}/status")]
        [SwaggerOperation(Summary = "Update order status")]
        [SwaggerResponse(StatusCodes.Status200OK, "Status updated successfully", typeof(SalesOrderResource))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid status or order not found")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while updating the order status")]
        public async Task<IActionResult> UpdateOrderStatus(
            [FromRoute] string orderId,
            [FromBody] UpdateOrderStatusRequest request)
        {
            try
            {
                var command = new UpdateOrderStatusCommand(
                    orderId,
                    request.NewStatus,
                    request.Reason
                );
            
                var updatedOrder = await salesOrderCommandService.Handle(command);
                var resource = SalesOrderResourceFromEntityAssembler.ToResourceFromEntity(updatedOrder);
            
                return Ok(resource);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { 
                    message = $"Order with ID {orderId} not found",
                    details = ex.Message 
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { 
                    message = "Invalid operation",
                    details = ex.Message 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { 
                    message = "An unexpected error occurred while updating the order status.",
                    details = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }
        /// <summary>
        /// Get the status of an order
        /// </summary>
        /// <param name="orderId">The ID of the order</param>
        /// <returns>Order status information</returns>
        [HttpGet("{orderId}/status")]
        [SwaggerOperation(Summary = "Get order status")]
        [SwaggerResponse(StatusCodes.Status200OK, "Order status retrieved successfully", typeof(OrderStatusResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found")]
        public async Task<IActionResult> GetOrderStatus([FromRoute] string orderId)
        {
            try
            {
                var order = await salesOrderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    return NotFound(new { 
                        message = $"Order with ID {orderId} not found"
                    });
                }

                var statusMessage = order.Status switch
                {
                    ESalesOrderStatuses.Received => "Your order has been received and is under review.",
                    ESalesOrderStatuses.Processing => "Your order is being processed.",
                    ESalesOrderStatuses.Confirmed => "Your order has been confirmed and is being prepared.",
                    ESalesOrderStatuses.Deliverying => "Your order is on its way.",
                    ESalesOrderStatuses.Arrived => "Your order has arrived.",
                    ESalesOrderStatuses.Canceled => "Your order has been canceled.",
                    _ => "Status unknown."
                };

                var lastStatusUpdate = order.StatusHistory
                    .OrderByDescending(h => h.Timestamp)
                    .FirstOrDefault();

                var response = new OrderStatusResponse
                {
                    OrderId = order.Id.ToString(),
                    Status = order.Status.ToString(),
                    Message = statusMessage,
                    LastUpdated = lastStatusUpdate?.Timestamp ?? DateTime.UtcNow,
                    Details = lastStatusUpdate?.Reason
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { 
                    message = "An error occurred while retrieving the order status.",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Get orders by supplier ID 
        /// </summary>
        /// <param name="supplierId">The ID of the supplier</param>
        /// <returns>List of orders for the specified supplier</returns>
        [HttpGet("supplier/{supplierId}")]
        [SwaggerOperation(Summary = "Get orders by supplier ID")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns the list of orders for the supplier", typeof(SupplierOrdersResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid supplier ID")]
        public async Task<IActionResult> GetSupplierOrders([FromRoute] string supplierId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(supplierId))return BadRequest(new { message = "Supplier ID is required" });

                var authenticatedUserId = User.FindFirst("sid")?.Value?? User.FindFirst(ClaimTypes.Sid)?.Value?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid")?.Value;

                if (string.IsNullOrEmpty(authenticatedUserId))return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "User ID not found in the token"
                });

                if (!string.Equals(supplierId, authenticatedUserId, StringComparison.OrdinalIgnoreCase))return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "You can only view your own orders"
                });

                var accountId = new AccountId(supplierId);
                var query = new GetAllSalesOrdersByBuyerIdQuery(accountId);
                var orders = (await salesOrderQueryService.Handle(query)).ToList();
                var response = new SupplierOrdersResponse
                    {
                        SupplierId = supplierId,
                        Orders = orders
                        .Select(SalesOrderResourceFromEntityAssembler.ToResourceFromEntity)
                        .ToList(),
                        TotalOrders = orders.Count,
                        Message = orders.Any() 
                            ? "Orders retrieved successfully" 
                            : "No orders found for the specified Supplier"
                    };

                return Ok(response);
            }catch (Exception ex){
                return StatusCode(StatusCodes.Status500InternalServerError, new 
                { 
                    message = "An error occurred while retrieving orders",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Get orders by Liquor Store Owner ID
        /// </summary>
        /// <param name="liquorStoreOwnerId">The ID of the Liquor Store Owner</param>
        /// <returns>List of orders for the specified Liquor Store Owner</returns>
        [HttpGet("liquor-store-owner/{liquorStoreOwnerId}")]
        [SwaggerOperation(Summary = "Get orders by Liquor Store Owner ID")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns the list of orders for the Liquor Store Owner", typeof(LiquorStoreOwnerOrdersResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Liquor Store Owner ID")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have permission to access this resource")]
        public async Task<IActionResult> GetOrdersByLiquorStoreOwner([FromRoute] string liquorStoreOwnerId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(liquorStoreOwnerId))return BadRequest(new { message = "Liquor Store Owner ID is required" });

                var authenticatedUserId = User.FindFirst("sid")?.Value
                                ?? User.FindFirst(ClaimTypes.Sid)?.Value
                                ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid")?.Value;

                if (string.IsNullOrEmpty(authenticatedUserId))return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "User ID not found in the token"
                });

                if (!string.Equals(liquorStoreOwnerId, authenticatedUserId, StringComparison.OrdinalIgnoreCase))return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "You can only view your own orders"
                });

                var query = new GetAllSalesOrdersQuery();
                var allOrders = (await salesOrderQueryService.Handle(query)).ToList();

                var orders = allOrders
                .Where(o => o.AccountId != null && 
                    string.Equals(o.AccountId.ToString(), liquorStoreOwnerId, StringComparison.OrdinalIgnoreCase))
                .ToList();

                var response = new LiquorStoreOwnerOrdersResponse
                {
                    LiquorStoreOwnerId = liquorStoreOwnerId,
                    Orders = orders
                    .Select(SalesOrderResourceFromEntityAssembler.ToResourceFromEntity)
                    .ToList(),
                    TotalOrders = orders.Count,
                    Message = orders.Any() 
                        ? "Orders retrieved successfully" 
                        : "No orders found for the specified Liquor Store Owner"
                };

                return Ok(response);
            }catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new 
                { 
                    message = "An error occurred while retrieving orders",
                    details = ex.Message
                });
            }
        }
    }
}
