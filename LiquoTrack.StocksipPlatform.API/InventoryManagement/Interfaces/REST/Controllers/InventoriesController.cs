using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Controllers;

/// <summary>
///     Controller class for handling inventory-related requests.
/// </summary>
/// <param name="inventoryCommandService">
///     The service for handling inventory-related commands.
/// </param>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available endpoints for inventories.")]
public class InventoriesController(
        IInventoryCommandService inventoryCommandService
    ) : ControllerBase
{
    /// <summary>
    ///     Endpoint to handle the deletion of an inventory by its ID.
    /// </summary>
    /// <param name="inventoryId">
    ///     The route parameter representing the unique identifier of the inventory to be deleted.
    /// </param>
    /// <returns>
    ///     A 204 No Content response if the inventory was deleted successfully, or a 404 Not Found response if the inventory was not found.
    /// </returns>
    [HttpDelete]
    [SwaggerOperation(
        Summary = "Delete inventory by ID.",
        Description = "Deletes an inventory by its unique identifier.",
        OperationId = "DeleteInventoryById")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Inventory deleted successfully.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Inventory with the specified ID was not found.")]
    public async Task<IActionResult> DeleteInventory([FromRoute] string inventoryId)
    {
        ObjectId inventoryObjId = new(inventoryId);
        var deleteInventoryCommand = new DeleteInventoryCommand(inventoryObjId);
        await inventoryCommandService.Handle(deleteInventoryCommand);
        return NoContent();
    }
}