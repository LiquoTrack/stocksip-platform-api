namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;

/// <summary>
///     Resource representing a subscription.
/// </summary>
public record SubscriptionResource(string PreferenceId, string InitPoint, string Message);