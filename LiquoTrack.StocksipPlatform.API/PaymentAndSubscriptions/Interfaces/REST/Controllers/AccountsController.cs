using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Controllers;

/// <summary>
///     Controller for handling account-related requests.
/// </summary>
/// <param name="accountQueryService">
///     The service for handling account-related queries.
/// </param>
[ApiController]
[Route("api/vi/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available endpoints for accounts.")]
public class AccountsController(IAccountQueryService accountQueryService) : ControllerBase
{
    /// <summary>
    ///     Endpoint to handle the retrieval of an account by its ID.
    /// </summary>
    /// <param name="accountId">
    ///     The ID of the account to retrieve.
    /// </param>
    /// <returns>
    ///     A 200 OK response with the details of the account, or a 404 Not Found response if the account does not exist.
    /// </returns>
    [HttpGet("{accountId}")]
    [SwaggerOperation(
        Summary = "Get Account By ID",
        Description = "Retrieves an account by its ID.",
        OperationId = "GetAccountById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Account returned successfully.", typeof(AccountResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Account not found.")]
    public async Task<IActionResult> GetAccountById([FromRoute] string accountId)
    {
        var account = await accountQueryService.Handle(new GetAccountByIdQuery(accountId));
        if (account is null) return NotFound($"Account with ID {accountId} not found.");
        var resource = AccountResourceFromEntityAssembler.ToResourceFromEntity(account);
        return Ok(resource);
    }
}