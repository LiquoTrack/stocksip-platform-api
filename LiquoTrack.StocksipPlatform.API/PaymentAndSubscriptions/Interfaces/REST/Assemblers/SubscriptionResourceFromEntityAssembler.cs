using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Assemblers;

/// <summary>
///     Static class to convert Subscription entity to SubscriptionResource.
/// </summary>
public class SubscriptionResourceFromEntityAssembler
{
    /// <summary>
    ///     Method to convert Subscription entity to SubscriptionResource.
    /// </summary>
    /// <param name="subscription">
    ///     The Subscription entity to convert.
    /// </param>
    /// <param name="preferenceId">}
    ///     The preference id of the subscription in MercadoPago.
    /// </param>
    /// <returns>
    ///     A SubscriptionResource object.
    /// </returns>
    public static SubscriptionResource ToResourceFromEntity(Subscription subscription, string preferenceId)
    {
        return new SubscriptionResource(
            subscription.Id.ToString(),
            preferenceId,
            subscription.Status.ToString());
    }
}