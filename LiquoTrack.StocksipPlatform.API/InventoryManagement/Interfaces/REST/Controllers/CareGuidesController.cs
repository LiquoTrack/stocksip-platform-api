using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;
using System;
using System.Linq;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerTag("Available Care Guide Endpoints")]
    public class CareGuidesController(ICareGuideCommandService careGuideCommandService, ICareGuideQueryService careGuideQueryService) : ControllerBase
    {
        [HttpGet("{careGuideId}")]
        [SwaggerOperation(
        Summary = "Get Care Guide by ID",
        Description = "Retrieves a care guide by its unique identifier.",
        OperationId = "GetCareGuideById")]
        [SwaggerResponse(StatusCodes.Status200OK, "Care Guide found!", typeof(CareGuideResource))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Care Guide not found...")]
        public async Task<IActionResult> GetCareGuideById(string careGuideId)
        {
            var getCareGuideByIdQuery = new GetCareGuideByIdQuery(careGuideId);
            var careGuide = await careGuideQueryService.Handle(getCareGuideByIdQuery);
            if (careGuide == null)
            {
                return NotFound($"Care guide with ID {careGuideId} not found...");
            }
            var resource = CareGuideResourceFromEntityAssembler.ToResourceFromEntity(careGuide);
            return Ok(resource);
        }
        [HttpPost("{accountId}/care-guides")]
        [SwaggerOperation(
            Summary = "Create a conservation guide by type of liquor",
            Description = "Creates a new conservation guide by type of liquor.",
            OperationId = "CreateCareGuideByTypeOfLiquor")]
        [SwaggerResponse(StatusCodes.Status201Created, "Care Guide created!", typeof(CareGuideResource))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request...")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Account not found...")]
        public async Task<IActionResult> CreateCareGuideByTypeOfLiquor([FromRoute] string accountId, [FromBody] CreateCareGuideByTypeOfLiquorResource resource)
        {
            if (string.IsNullOrWhiteSpace(accountId))
            {
                return BadRequest("AccountId is required.");
            }
            if (resource is null)
            {
                return BadRequest("Request body is required.");
            }

            var productType = resource.TypeOfLiquor;

            if (!Enum.IsDefined(typeof(EProductTypes), productType))
            {
                return BadRequest("Invalid TypeOfLiquor value.");
            }

            var (minTemp, maxTemp, storagePlace, recommendation) = productType switch
            {
                EProductTypes.Whiskeys or EProductTypes.Rums or EProductTypes.Tequilas =>
                    (15, 20, "Cool, dark place", $"Store {productType} at a consistent temperature between 15-20°C"),
                EProductTypes.Wines =>
                    (12, 18, "Wine cellar or dark place", $"Store {productType} horizontally in a dark place between 12-18°C"),
                EProductTypes.Beers =>
                    (4, 10, "Refrigerator", $"Store {productType} refrigerated between 4-10°C"),
                _ =>
                    (0, 25, "Cool, dry place", $"Store {productType} in a cool, dry place")
            };

            var createCommand = new CreateCareGuideWithoutProductIdCommand(
                careGuideId: Guid.NewGuid().ToString(),
                accountId: accountId,
                productAssociated: productType.ToString(),
                title: $"Care Guide for {productType}",
                summary: $"This is a care guide for {productType} products.",
                recommendedMinTemperature: minTemp,
                recommendedMaxTemperature: maxTemp,
                recommendedPlaceStorage: storagePlace,
                generalRecommendation: recommendation
            );

            var careGuides = await careGuideCommandService.Handle(createCommand);
            var createdCareGuide = careGuides.FirstOrDefault();
            if (createdCareGuide == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create care guide");
            }
            var careGuideResource = CareGuideResourceFromEntityAssembler.ToResourceFromEntity(createdCareGuide);
            return CreatedAtAction(nameof(GetCareGuideById), new { accountId, careGuideId = createdCareGuide.Id }, careGuideResource);
        }

        [HttpGet("{accountId}/care-guides/{productType}")]
        [SwaggerOperation(
            Summary = "Get Care Guide by Type of Liquor",
            Description = "Retrieves a care guide by its type of liquor.",  
            OperationId = "GetCareGuideByTypeOfLiquor")]
        [SwaggerResponse(StatusCodes.Status200OK, "Care Guide found!", typeof(CareGuideResource))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Care Guide not found...")]
        public async Task<IActionResult> GetCareGuideByTypeOfLiquor([FromRoute] string accountId, [FromRoute] EProductTypes productType)
        {
            var getCareGuideByTypeOfLiquorQuery = new GetCareGuideByTypeOfLiquorQuery(accountId, productType);
            var careGuide = await careGuideQueryService.Handle(getCareGuideByTypeOfLiquorQuery);
            if (careGuide == null)
            {
                return NotFound($"Care guide for {productType} not found...");
            }
            var resource = CareGuideResourceFromEntityAssembler.ToResourceFromEntity(careGuide);
            return Ok(resource);
        }


        [HttpPut("{careGuideId}")]
        [SwaggerOperation(
            Summary = "Update Care Guide",
            Description = "Update Recommendations for A Specific Care Guide.",
            OperationId = "UpdateCareGuide")]
        [SwaggerResponse(StatusCodes.Status200OK, "Care Guide updated successfully!", typeof(CareGuideResource))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Care Guide could not be updated...")]
        public async Task<IActionResult> UpdateCareGuideRecommendations([FromBody] UpdateCareGuideResource resource, [FromRoute] string careGuideId)
        {
            var updateCareGuideCommand = UpdateCareGuideCommandFromResourceAssembler.ToCommandFromResource(resource, careGuideId);
            var updatedCareGuides = await careGuideCommandService.Handle(updateCareGuideCommand);
            var updatedCareGuide = updatedCareGuides.FirstOrDefault();
            if (updatedCareGuide is null)
            {
                return BadRequest($"Failed to update care guide with ID {careGuideId}. Please check the provided data.");
            }
            var updatedCareGuideResource = CareGuideResourceFromEntityAssembler.ToResourceFromEntity(updatedCareGuide);
            return Ok(updatedCareGuideResource);
        }

        [HttpDelete("{careGuideId}")]
        [SwaggerOperation(
            OperationId = "DeleteCareGuide")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Care Guide deleted successfully!")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Care Guide not found...")]
        public async Task<IActionResult> DeleteCareGuide([FromRoute] string careGuideId)
        {
            var deleteCareGuideCommand = new DeleteCareGuideCommand(careGuideId);
            await careGuideCommandService.Handle(deleteCareGuideCommand);
            return Ok(new { Message = $"Care Guide with ID {careGuideId} deleted successfully." });
        }

        [HttpPut("{careGuideId}/deallocations")]
        [SwaggerOperation(
            Summary = "Unassing a Care Guide",
            Description = "Unassign a Care Guide From Its Current Product.",
            OperationId = "DeallocateCareGuide")]
        [SwaggerResponse(StatusCodes.Status200OK, "Care Guide unassigned successfully!")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Care Guide could not be unassign...")]
        public async Task<IActionResult> DeallocateCareGuide([FromRoute] string careGuideId)
        {
            var unassignCareGuideCommand = new UnassignCareGuideCommand(careGuideId);
            await careGuideCommandService.Handle(unassignCareGuideCommand);
            return Ok(new
            { Message = $"Care Guide with ID {careGuideId} unassigned from its current product successfully." });
        }

        [HttpPut("{careGuideId}/allocations/{productId}")]
        [SwaggerOperation(
            Summary = "Assign a Care Guide",
            Description = "Assign a Care Guide To a Specific Product.",
            OperationId = "AllocateCareGuideToProduct")]
        [SwaggerResponse(StatusCodes.Status200OK, "Care Guide assigned successfully!")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Care Guide could not be assigned...")]
        public async Task<IActionResult> AllocateCareGuideToProduct([FromRoute] string careGuideId, [FromRoute] string productId)
        {
            var assignCareGuideCommand = new AssignCareGuideToProductCommand(careGuideId, productId);
            await careGuideCommandService.Handle(assignCareGuideCommand);
            return Ok(new
            { Message = $"Care Guide with ID {careGuideId} assigned to product with ID {productId} successfully." });
        }
    }
}
