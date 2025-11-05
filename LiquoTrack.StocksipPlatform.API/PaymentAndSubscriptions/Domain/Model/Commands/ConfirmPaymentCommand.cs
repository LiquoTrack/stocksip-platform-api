namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Commands;

/// <summary>
///     Command to confirm a payment.
/// </summary>
public record ConfirmPaymentCommand(string AccountId, string Status);