namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Queries;

/// <summary>
///     Query to retrieve the status of a subscription by its preference ID.
/// </summary>
/// <param name="PreferenceId">
///     The unique identifier of the preference whose subscription status is to be retrieved.
/// </param>
public record GetSubscriptionStatusByPreferenceIdQuery(string PreferenceId);