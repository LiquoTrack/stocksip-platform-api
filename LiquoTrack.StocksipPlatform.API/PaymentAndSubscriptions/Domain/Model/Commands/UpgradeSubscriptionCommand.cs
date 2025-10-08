namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Commands;

/// <summary>
///     Command to upgrade a subscription for a given account.
/// </summary>
/// <param name="AccountId">
///     The unique identifier of the account whose subscription is to be upgraded.
/// </param>
/// <param name="NewPlanId">
///     The unique identifier of the new plan to which the subscription is to be upgraded.
/// </param>
public record UpgradeSubscriptionCommand(string AccountId, string NewPlanId);