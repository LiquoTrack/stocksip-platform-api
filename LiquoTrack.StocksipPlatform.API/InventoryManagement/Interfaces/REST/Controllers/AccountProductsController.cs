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
///     Controller for handling product-related requests.
/// </summary>
/// <param name="productQueryService">
///     The service for handling product-related queries. 
/// </param>
[ApiController]
[Route("api/v1/accounts/{accountId}/products")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Accounts")]
public class AccountProductsController(
        IProductQueryService productQueryService   
    ) : ControllerBase
{
    /// <summary>
    ///     Endpoint to handle the retrieval of all products for a specific account.
    /// </summary>
    /// <param name="accountId">
    ///     The route parameter representing the unique identifier of the account for which to retrieve products.   
    /// </param>
    /// <returns>
    ///     A list of products for the specified account, or a 404 Not Found response if no products are found for the given account ID. 
    /// </returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get All Products by Account ID",
        Description = "Retrieves a list of products by a specific Account ID.",
        OperationId = "GetAllProductsByAccountId")]
    [SwaggerResponse(StatusCodes.Status200OK, "Products found!", typeof(List<ProductResource>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Products not found for the give Account ID...")]
    public async Task<IActionResult> GetAllProductsByAccountId([FromRoute] string accountId)
    {
        var targetAccountId = new AccountId(accountId);
        var getAllProductsByAccountIdQuery = new GetAllProductsByAccountIdQuery(targetAccountId);
        var products = await productQueryService.Handle(getAllProductsByAccountIdQuery);
        var productsList = products.ToList();
        if (productsList.Count == 0) return NotFound($"No products found for account ID {accountId}.");
        var resources = productsList
            .Select(ProductResourceFromEntityAssembler.ToResourceFromEntity)
            .ToList();
        return Ok(resources);
    }
}