using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Controllers;

/// <summary>
///     Controller for handling subscription-related requests.
/// </summary>
/// <param name="subscriptionsCommandService">
///     The service for handling subscription-related commands.
/// </param>
[ApiController]
[Route("api/v1/accounts/{accountId}/subscriptions")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Accounts")]
public class AccountSubscriptionsController(ISubscriptionsCommandService subscriptionsCommandService) : ControllerBase
{
    
    /// <summary>
    ///     The service for handling subscription-related commands.
    /// </summary>
    /// <param name="accountId">
    ///     The route parameter representing the unique identifier of the account for which to create a subscription.
    /// </param>
    /// <param name="resource">
    ///     The request body containing the subscription details.  
    /// </param>
    /// <returns>
    ///     The newly created subscription.
    /// </returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new subscription for an account.",
        Description = "Initializes a new subscription for the specified account.",
        OperationId = "CreateSubscription"
        )]
    [SwaggerResponse(StatusCodes.Status201Created, "Subscription created successfully.", typeof(SubscriptionResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Subscription could not be created.")]
    public async Task<IActionResult> CreateSubscription([FromRoute] string accountId, [FromBody] InitialSubscriptionResource resource)
    {
        var command = InitialSubscriptionCommandFromResourceAssembler.FromCommandToEntity(resource, accountId);
        var (preferenceId, initPoint) = await subscriptionsCommandService.Handle(command);

        if (preferenceId is null && initPoint is null)
        {
            var freeSubscription = new SubscriptionResource(null, null, "Free plan activated successfully.");

            return CreatedAtAction(nameof(CreateSubscription), freeSubscription);
        }

        var subscriptionResource =
            SubscriptionResourceFromEntityAssembler.ToResourceFromEntity(preferenceId!, initPoint!, message: "Processing subscription request..");

        return CreatedAtAction(nameof(CreateSubscription), subscriptionResource);
    }
    
}