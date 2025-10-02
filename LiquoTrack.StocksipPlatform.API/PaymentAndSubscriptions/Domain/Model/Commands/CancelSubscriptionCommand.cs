namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Commands;

/// <summary>
///     Command to cancel a subscription for a given account.
/// </summary>
/// <param name="AccountId">
///     The unique identifier of the account whose subscription is to be canceled.
/// </param>
public record CancelSubscriptionCommand(string AccountId);