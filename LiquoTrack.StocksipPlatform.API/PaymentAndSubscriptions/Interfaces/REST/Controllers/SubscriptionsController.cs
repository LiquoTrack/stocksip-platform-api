using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Pipeline.Middleware.Attributes;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Subscriptions")]
public class SubscriptionsController(
    ISubscriptionsCommandService subscriptionsCommandService,
    ISubscriptionQueryService subscriptionQueryService,
    IConfiguration configuration) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Confirm subscription",
        Description = "Endpoint to confirm a subscription based on the provided notification from MercadoPago.",
        OperationId = "ConfirmedSubscription")]
    [SwaggerResponse(StatusCodes.Status200OK, "Subscription confirmed successfully.", typeof(SubscriptionConfirmResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Subscription could not be confirmed.")]
    public async Task<IActionResult> ConfirmedSubscription([FromServices] ILogger<SubscriptionsController> logger, [FromQuery] string? testSignature)
    {
        try
        {
            // --- Log headers completos ---
            logger.LogInformation("Headers recibidos: {Headers}", Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));

            // --- Obtener x-signature ---
            var signatureHeader = testSignature ?? Request.Headers["x-signature"].ToString();
            if (string.IsNullOrWhiteSpace(signatureHeader))
            {
                logger.LogWarning("Falta x-signature en el header");
                return BadRequest("Falta x-signature");
            }

            var xRequestId = Request.Headers["x-request-id"].ToString();
            logger.LogInformation("x-request-id: {XRequestId}", xRequestId);

            // --- Leer body completo ---
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            logger.LogInformation("Body recibido: {Body}", body);

            if (string.IsNullOrWhiteSpace(body))
            {
                logger.LogWarning("Body vacío");
                return BadRequest("Body vacío");
            }

            // --- Extraer data.id ---
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            if (!root.TryGetProperty("data", out var dataElement) || !dataElement.TryGetProperty("id", out var dataIdElement))
            {
                logger.LogWarning("No se encontró data.id en el body");
                return BadRequest("No se encontró data.id");
            }

            var dataId = dataIdElement.GetString() ?? "";
            logger.LogInformation("data.id: {DataId}", dataId);

            // --- Validar firma ---
            var signatureValid = ValidateSignature(signatureHeader, xRequestId, dataId, logger);
            logger.LogInformation("Resultado de validación de firma: {SignatureValid}", signatureValid);

            if (!signatureValid)
            {
                return BadRequest("Invalid signature");
            }

            // --- Procesar suscripción ---
            var webHookCommand = WebhookPaymentCommandFromResourceAssembler.ToCommandFromResource(dataId);
            var subscription = await subscriptionsCommandService.Handle(webHookCommand);
            logger.LogInformation("Suscripción procesada exitosamente: {DataId}", dataId);

            return Ok(new { message = "Subscription processed successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error procesando la suscripción en producción");
            return BadRequest(new { error = ex.Message });
        }
    }

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

    private bool ValidateSignature(string signatureHeader, string xRequestId, string dataId, ILogger logger)
    {
        try
        {
            var parts = signatureHeader.Split(',');
            string ts = "", v1 = "";
            foreach (var part in parts)
            {
                var kv = part.Split('=', 2);
                if (kv.Length != 2) continue;
                if (kv[0].Trim() == "ts") ts = kv[1].Trim();
                if (kv[0].Trim() == "v1") v1 = kv[1].Trim();
            }

            logger.LogInformation("Parsed x-signature: ts={Ts}, v1={V1}", ts, v1);

            var mercadoPagoSecret = configuration["MercadoPagoSettings:WebhookSecret"];
            var manifest = $"id:{dataId};request-id:{xRequestId};ts:{ts};";
            logger.LogInformation("Manifest generado para HMAC: {Manifest}", manifest);

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(mercadoPagoSecret));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(manifest));
            var hashHex = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            logger.LogInformation("Hash generado: {HashHex}, Signature recibida: {V1}", hashHex, v1);

            var isValid = hashHex == v1;
            logger.LogInformation("Firma válida: {IsValid}", isValid);

            return isValid;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validando la firma");
            return false;
        }
    }
}
