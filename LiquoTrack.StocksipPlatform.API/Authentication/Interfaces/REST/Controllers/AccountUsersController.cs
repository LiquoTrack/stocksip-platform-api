using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Transform;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Controllers;

/// <summary>
///     Controller for handling user-related requests.
/// </summary>
[ApiController]
[Route("api/v1/accounts/{accountId}")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Accounts")]
public class AccountUsersController(IUserCommandService userCommandService) : ControllerBase
{
    /// <summary>
    ///     Method to register a sub user for an account.
    /// </summary>
    /// <param name="accountId">
    ///     The ID of the account for which to register a sub user.
    /// </param>
    /// <param name="resource">
    ///     A resource containing the details for registering a sub user.
    /// </param>
    /// <returns>
    ///     A 200 OK response with the details of the newly registered sub user, or a 400 Bad Request response if the sub user could not be registered.  
    /// </returns>
    [HttpPost("users")]
    [SwaggerOperation(
        summary: "Register a sub user for an account.",
        description: "Register a sub user for an account."
        )]
    [SwaggerResponse(StatusCodes.Status200OK, "Sub user registered successfully.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.")]
    public async Task<IActionResult> RegisterUser([FromRoute] string accountId, [FromBody] RegisterSubUserResource resource)
    {
        try
        {
            var registerSubUserCommand =
                RegisterSubUserCommandFromResourceAssembler.ToCommandFromResource(resource, accountId);
            var user = await userCommandService.Handle(registerSubUserCommand);
            return Ok(new { message = "Sub User registered successfully" });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }
}