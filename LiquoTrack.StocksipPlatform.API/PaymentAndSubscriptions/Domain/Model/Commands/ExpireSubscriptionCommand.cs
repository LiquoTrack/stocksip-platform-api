namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Commands;

/// <summary>
///     Command to expire a subscription for a given account.
/// </summary>
/// <param name="AccountId">
///     The unique identifier of the account whose subscription is to be expired.
/// </param>
public record ExpireSubscriptionCommand(string AccountId);