using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Security.Claims;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.ACL;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using Microsoft.AspNetCore.Authorization;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Controllers
{
    [ApiController]
    [Route("api/v1/orders")]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerTag("Available Sales Order Endpoints")]
    public class SalesOrderController : ControllerBase
    {
        private readonly ISalesOrderCommandService _salesOrderCommandService;
        private readonly ISalesOrderQueryService _salesOrderQueryService;
        private readonly ISalesOrderRepository _salesOrderRepository;
        private readonly ISalesOrderFacade _salesOrderFacade;
        private readonly AppDbContext _dbContext;
        private readonly IUserQueryService _userQueryService;

        public SalesOrderController(
            ISalesOrderCommandService salesOrderCommandService,
            ISalesOrderQueryService salesOrderQueryService,
            ISalesOrderRepository salesOrderRepository,
            ISalesOrderFacade salesOrderFacade,
            AppDbContext dbContext,
            IUserQueryService userQueryService)
        {
            _salesOrderCommandService = salesOrderCommandService;
            _salesOrderQueryService = salesOrderQueryService;
            _salesOrderRepository = salesOrderRepository;
            _salesOrderFacade = salesOrderFacade;
            _dbContext = dbContext;
            _userQueryService = userQueryService;
        }
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
                    return Unauthorized("User not authenticated");

                var order = await _salesOrderCommandService.GeneratePurchaseOrder(request, accountId);
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

        [HttpPost("from-procurement/completed")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Create a Sales Order from a completed Procurement order payload")]
        [SwaggerResponse(StatusCodes.Status201Created, "Sales order created successfully", typeof(SalesOrderResource))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request data")]
        public async Task<IActionResult> CreateFromProcurement([FromBody] ProcurementOrderCompletedResource payload)
        {
            if (payload == null || payload.Items == null || payload.Items.Count == 0)
                return BadRequest("Payload or items cannot be null/empty");

            try
            {
                var command = ProcurementCompletedToGenerateSalesOrderCommandAssembler.ToCommand(payload);
                var order = await _salesOrderCommandService.Handle(command);
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

        [HttpPost("from-procurement/{purchaseOrderId}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Convert a completed purchase order to a sales order")]
        [SwaggerResponse(StatusCodes.Status201Created, "Sales order created from purchase order", typeof(SalesOrderResource))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid purchase order ID")]
        public async Task<IActionResult> CreateFromPurchaseOrder([FromRoute] string purchaseOrderId)
        {
            try
            {
                var salesOrderId = await _salesOrderFacade.ConvertPurchaseOrderToSalesOrderAsync(purchaseOrderId);
                var order = await _salesOrderRepository.GetByIdAsync(salesOrderId);

                if (order == null)
                    return NotFound($"Sales order {salesOrderId} not found after conversion");

                var resource = SalesOrderResourceFromEntityAssembler.ToResourceFromEntity(order);
                return CreatedAtAction(nameof(GetOrderById), new { id = salesOrderId }, resource);
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
            var order = await _salesOrderRepository.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            var resource = SalesOrderResourceFromEntityAssembler.ToResourceFromEntity(order);
            return Ok(resource);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all orders")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns all orders", typeof(IEnumerable<SalesOrderResource>))]
        public async Task<IActionResult> GetAllOrders()
        {
            var query = new GetAllSalesOrdersQuery();
            var orders = await _salesOrderQueryService.Handle(query);
            var resources = orders.Select(SalesOrderResourceFromEntityAssembler.ToResourceFromEntity);
            return Ok(resources);
        }

        [HttpGet("supplier/{supplierId}")]
        [SwaggerOperation(Summary = "Get orders by supplier ID")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns the list of orders for the supplier", typeof(SupplierOrdersResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid supplier ID")]
        public async Task<IActionResult> GetSupplierOrders([FromRoute] string supplierId)
        {
            if (string.IsNullOrWhiteSpace(supplierId))
                return BadRequest(new { message = "Supplier ID is required" });

            try
            {
                var authenticatedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                        ?? User.FindFirst("sid")?.Value;

                var authenticatedAccountId = User.FindFirst("accountId")?.Value
                                        ?? User.FindFirst("accid")?.Value;

                var matchesAccount = !string.IsNullOrEmpty(authenticatedAccountId) && string.Equals(supplierId, authenticatedAccountId, StringComparison.OrdinalIgnoreCase);
                var matchesUser = !string.IsNullOrEmpty(authenticatedUserId) && string.Equals(supplierId, authenticatedUserId, StringComparison.OrdinalIgnoreCase);

                if (!matchesAccount && !matchesUser)
                {
                    if (!string.IsNullOrEmpty(authenticatedUserId))
                    {
                        var user = await _userQueryService.Handle(new GetUserByIdQuery(authenticatedUserId));
                        var userAccountId = user?.AccountId?.GetId;
                        if (!string.IsNullOrEmpty(userAccountId) && string.Equals(supplierId, userAccountId, StringComparison.OrdinalIgnoreCase))
                            matchesAccount = true;
                    }

                    if (!matchesAccount)
                        return StatusCode(StatusCodes.Status403Forbidden, new { message = "You can only view your own orders (account)" });
                }

                var accountId = new AccountId(supplierId);
                var query = new GetAllSalesOrdersBySupplierIdQuery(accountId);
                var orders = (await _salesOrderQueryService.Handle(query)).ToList();

                var response = new SupplierOrdersResponse
                {
                    SupplierId = supplierId,
                    Orders = orders.Select(SalesOrderResourceFromEntityAssembler.ToResourceFromEntity).ToList(),
                    TotalOrders = orders.Count,
                    Message = orders.Any() ? "Orders retrieved successfully" : "No orders found for the specified Supplier"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while retrieving orders",
                    details = ex.Message
                });
            }
        }

        [HttpGet("liquor-store-owner/{liquorStoreOwnerId}")]
        [SwaggerOperation(Summary = "Get orders by Liquor Store Owner ID")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns the list of orders for the Liquor Store Owner", typeof(LiquorStoreOwnerOrdersResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Liquor Store Owner ID")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have permission to access this resource")]
        public async Task<IActionResult> GetOrdersByLiquorStoreOwner([FromRoute] string liquorStoreOwnerId)
        {
            if (string.IsNullOrWhiteSpace(liquorStoreOwnerId))
                return BadRequest(new { message = "Liquor Store Owner ID is required" });

            var authenticatedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                    ?? User.FindFirst("sid")?.Value;

            if (string.IsNullOrEmpty(authenticatedUserId))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "User ID not found in the token" });

            if (!string.Equals(liquorStoreOwnerId, authenticatedUserId, StringComparison.OrdinalIgnoreCase))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You can only view your own orders" });

            var query = new GetAllSalesOrdersQuery();
            var allOrders = (await _salesOrderQueryService.Handle(query)).ToList();
            var orders = allOrders
                .Where(o => o.AccountId != null && string.Equals(o.AccountId.GetId, liquorStoreOwnerId, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var response = new LiquorStoreOwnerOrdersResponse
            {
                LiquorStoreOwnerId = liquorStoreOwnerId,
                Orders = orders.Select(SalesOrderResourceFromEntityAssembler.ToResourceFromEntity).ToList(),
                TotalOrders = orders.Count,
                Message = orders.Any() ? "Orders retrieved successfully" : "No orders found for the specified Liquor Store Owner"
            };

            return Ok(response);
        }
    }
}
