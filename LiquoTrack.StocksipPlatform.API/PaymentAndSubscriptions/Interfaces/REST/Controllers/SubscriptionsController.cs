using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Controllers;

/// <summary>
///     Controller for handling subscription-related requests.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Subscriptions")]
public class SubscriptionsController(
    ISubscriptionsCommandService subscriptionsCommandService,
    ISubscriptionQueryService subscriptionQueryService,
    IConfiguration configuration) : ControllerBase
{
    /// <summary>
    ///     Method to handle the confirmation of a subscription.
    /// </summary>
    /// <param name="resource">
    ///     The resource containing the details for confirming a subscription.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation.
    /// </returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Confirm subscription",
        Description = "Endpoint to confirm a subscription based on the provided notification from MercadoPago.",
        OperationId = "ConfirmedSubscription")]
    [SwaggerResponse(StatusCodes.Status200OK, "Subscription confirmed successfully.", typeof(SubscriptionConfirmResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Subscription could not be confirmed.")]
    public async Task<IActionResult> ConfirmedSubscription([FromBody] SubscriptionConfirmResource resource)
    {
        if (!Request.Headers.TryGetValue("x-signature", out var signatureHeader))
        {
            return BadRequest("Falta x-signature");
        }

        var xRequestId = Request.Headers["x-request-id"].ToString();
        var dataId = Request.Query["data.id"].ToString();

        if (string.IsNullOrEmpty(xRequestId) || string.IsNullOrEmpty(dataId))
            return BadRequest("Missing x-request-id or data.id");

        var parts = signatureHeader.ToString().Split(',');
        string ts = "", v1 = "";
        foreach (var part in parts)
        {
            var kv = part.Split('=', 2);
            if (kv.Length != 2) continue;
            if (kv[0].Trim() == "ts") ts = kv[1].Trim();
            if (kv[0].Trim() == "v1") v1 = kv[1].Trim();
        }

        var mercadoPagoSecret = configuration["MercadoPagoSettings:WebhookSecret"];
        var manifest = $"id:{dataId};request-id:{xRequestId};ts:{ts};";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(mercadoPagoSecret));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(manifest));
        var hashHex = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        if (hashHex != v1)
            return BadRequest("Invalid signature");
        
        try
        {
            var command = SubscriptionConfirmCommandFromResourceAssembler.ToCommandFromResource(resource);
            await subscriptionsCommandService.Handle(command);
            
            return Ok(new { message = "Processing subscription confirmation.." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    ///     Method to handle the retrieval of a subscription's status by its preference ID.'
    /// </summary>
    /// <param name="preferenceId">
    ///     The ID of the subscription to retrieve.
    /// </param>   
    /// <returns>
    ///     A task representing the asynchronous operation. The task result contains the subscription status.
    /// </returns>
    [HttpGet("status")]
    [SwaggerOperation(
        Summary = "Get the subscription status by preference ID",
        Description = "Retrieves the status of a subscription based on the provided preference ID.",
        OperationId = "GetSubscriptionStatus"
        )]
    [SwaggerResponse(StatusCodes.Status200OK, "Subscription status returned successfully.", typeof(SubscriptionStatusResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Subscription status could not be retrieved.")]
    public async Task<IActionResult> GetSubscriptionStatus([FromQuery] string preferenceId)
    {
        var getSubscriptionStatusByPreferenceIdQuery = new GetSubscriptionStatusByPreferenceIdQuery(preferenceId);
        var subscriptionStatus = await subscriptionQueryService.Handle(getSubscriptionStatusByPreferenceIdQuery);
        var response = SubscriptionStatusResourceFromEntityAssembler.ToResourceFromEntity(subscriptionStatus);
        return Ok(response);
    }
}