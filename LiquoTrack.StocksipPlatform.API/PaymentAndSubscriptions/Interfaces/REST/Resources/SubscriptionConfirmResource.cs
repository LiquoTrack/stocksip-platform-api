using MercadoPago.Resource.Payment;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;

/// <summary>
///     Resource class for MercadoPago notifications.
/// </summary>
/// <param name="PreferenceId">
///     The preference ID associated with the notification.
/// </param>
/// <param name="Status">
///     The status of the notification.
/// </param>
public record SubscriptionConfirmResource(string PreferenceId, string Status);