using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Pipeline.Middleware.Attributes;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Controllers;

/// <summary>
///     Controller for user password recovery operations.
/// </summary>
[ApiController]
[Route("api/v1/users")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Users")]
public class UserPasswordRecoveryController(
    IUserCommandService userCommandService) : ControllerBase
{
    /// <summary>
    ///     Method to send a recovery code to the email service.
    /// </summary>
    /// <param name="resource">
    ///     Resource containing the details for sending a recovery code.
    /// </param>
    /// <returns>
    ///     A 200 OK response with the details of the recovery code sent, or a 400 Bad Request response if the recovery code could not be sent. 
    /// </returns>
    [HttpPost("recovery-code")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Send a recovery code to the email service",
        Description = "Send a recovery code to the email service",
        OperationId = "SendRecoveryCode"
        )]
    [SwaggerResponse(StatusCodes.Status200OK, "Recovery code sent successfully.", typeof(object))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(string))]
    public async Task<IActionResult> RecoveryCode([FromBody] SendCodeToRecoverPasswordResource resource)
    {
        var command = SendCodeToRecoverCommandFromResourceAssembler.ToCommandFromResource(resource);
        await userCommandService.Handle(command);
        return Ok(new { Message = "Recovery code sent successfully" });
    }
}