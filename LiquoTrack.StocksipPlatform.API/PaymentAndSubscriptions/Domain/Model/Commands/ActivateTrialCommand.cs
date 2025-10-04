namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Commands;

/// <summary>
///     The command to activate a trial subscription for a given account.
/// </summary>
/// <param name="AccountId">
///     The unique identifier of the account for which the trial subscription is to be activated.
/// </param>
public record ActivateTrialCommand(string AccountId);