using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;
using MongoDB.Bson;
using MongoDB.Driver;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using System.Security.Claims;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Controllers
{
    [ApiController]
    [Route("api/v1/orders")]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerTag("Available Sales Order Endpoints")]
    public class SalesOrderController(
        ISalesOrderCommandService salesOrderCommandService,
        ISalesOrderQueryService salesOrderQueryService,
        AppDbContext dbContext)
        : ControllerBase
    {
        [HttpPost]
        [SwaggerOperation(Summary = "Create order")]
        [SwaggerResponse(StatusCodes.Status201Created, "Order created", typeof(CreateSalesOrderResource))]
        public async Task<IActionResult> CreateOrder([FromBody] CreateSalesOrderResource body)
        {
            var accountId = User.FindFirst("accountId")?.Value
                             ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? User.FindFirst(ClaimTypes.Name)?.Value
                             ?? string.Empty;

            var cmd = CreateSalesOrderCommandFromResourceAssembler.ToCommandFromResource(body, accountId);
            var created = await salesOrderCommandService.Handle(cmd);

            var resource = SalesOrderResourceFromEntityAssembler.ToResourceFromEntity(created);

            return Created($"/api/v1/orders/{created.Id}", resource);
        }
    }
}
