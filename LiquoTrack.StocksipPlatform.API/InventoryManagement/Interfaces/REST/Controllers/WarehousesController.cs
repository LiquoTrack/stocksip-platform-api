using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Controllers;

/// <summary>
///     Controller for handling warehouse-related requests.
/// </summary>
/// <param name="warehouseCommandService">
///     The service for handling warehouse-related commands.
/// </param>
/// <param name="warehouseQueryService">
///     The service for handling warehouse-related queries.
/// </param>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available endpoints for warehouses.")]
public class WarehousesController(
        IWarehouseCommandService warehouseCommandService,
        IWarehouseQueryService warehouseQueryService
    ) : ControllerBase
{
    /// <summary>
    ///     Endpoint to handle the retrieval of a warehouse by its ID.
    /// </summary>
    /// <param name="id">
    ///     The route parameter representing the unique identifier of the warehouse to be retrieved.
    /// </param>
    /// <returns>
    ///     The warehouse with the specified ID, or a 404 Not Found response if the warehouse does not exist.
    /// </returns>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get warehouse by ID.",
        Description = "Retrieves a warehouse by its unique identifier.",
        OperationId = "GetWarehouseById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Warehouse returned successfully.", typeof(WarehouseResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Warehouse with the specified ID was not found.")]
    public async Task<IActionResult> GetWarehouseById([FromRoute] string id)
    {
        var getWarehouseById = new GetWarehouseByIdQuery(id);
        var warehouse = await warehouseQueryService.Handle(getWarehouseById);
        if (warehouse is null) return NotFound($"Warehouse with ID {id} not found.");
        var warehouseResource = WarehouseResourceFromEntityAssembler.ToResourceFromEntity(warehouse);
        return Ok(warehouseResource);
    }

    /// <summary>
    ///     Endpoint to handle the registration of a new warehouse.
    /// </summary>
    /// <param name="resource">
    ///     The request body containing the details of the warehouse to be registered.
    /// </param>
    /// <returns>
    ///     A 201 Created response with the details of the newly registered warehouse, or a 400 Bad Request response if the warehouse could not be registered.   
    /// </returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Register a new warehouse.",
        Description = "Registers a new warehouse with the provided details.",
        OperationId = "RegisterWarehouse")]
    [SwaggerResponse(StatusCodes.Status201Created, "Warehouse registered successfully.", typeof(WarehouseResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input data.")]
    public async Task<IActionResult> RegisterWarehouse([FromBody] RegisterWarehouseResource resource)
    {
        var registerWarehouseCommand = RegisterWarehouseCommandFromResourceAssembler.ToCommandFromResource(resource);
        var warehouse = await warehouseCommandService.Handle(registerWarehouseCommand);
        if (warehouse is null) return BadRequest("Warehouse could not be registered.");
        var warehouseResource = WarehouseResourceFromEntityAssembler.ToResourceFromEntity(warehouse);
        return CreatedAtAction(nameof(GetWarehouseById), new { id = warehouse.Id.ToString() }, warehouseResource);
    }

    /// <summary>
    ///     Endpoint to handle the update of a warehouse.
    /// </summary>
    /// <param name="id">
    ///     The route parameter representing the unique identifier of the warehouse to be updated.
    /// </param>
    /// <param name="resource">
    ///     The request body containing the updated warehouse information.
    /// </param>
    /// <returns>
    ///     A 200 OK response with the updated warehouse details, or a 400 Bad Request response if the warehouse could not be updated.
    /// </returns>
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Update warehouse information.",
        Description = "Updates the information of a warehouse with the provided details.",
        OperationId = "UpdateWarehouse")]
    [SwaggerResponse(StatusCodes.Status200OK, "Warehouse updated successfully.", typeof(WarehouseResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input data.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Warehouse with the specified ID was not found.")]
    public async Task<IActionResult> UpdateWarehouse([FromRoute] string id,
        [FromBody] UpdateWarehouseInformationResource resource)
    {
        var updateWarehouseCommand = UpdateWarehouseInformationCommandFromResourceAssembler.ToCommandFromResource(id, resource);
        var warehouse = await warehouseCommandService.Handle(updateWarehouseCommand);
        if (warehouse is null) return NotFound($"Warehouse with ID {id} not found.");
        var warehouseResource = WarehouseResourceFromEntityAssembler.ToResourceFromEntity(warehouse);
        return Ok(warehouseResource);
    }

    /// <summary>
    ///     Endpoint to handle the deletion of a warehouse by its ID.
    /// </summary>
    /// <param name="id">
    ///     The route parameter representing the unique identifier of the warehouse to be deleted.
    /// </param>
    /// <returns>
    ///     A 204 No Content response if the warehouse was successfully deleted, or a 404 Not Found response if the warehouse to delete could not be found.   
    /// </returns>
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Delete a warehouse.",
        Description = "Deletes a warehouse with the specified ID.",
        OperationId = "DeleteWarehouse")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Warehouse deleted successfully.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Warehouse with the specified ID was not found.")]
    public async Task<IActionResult> DeleteWarehouse([FromRoute] string id)
    {
        var deleteWarehouseCommand = new DeleteWarehouseCommand(id);
        await warehouseCommandService.Handle(deleteWarehouseCommand);
        return NoContent();
    }
}