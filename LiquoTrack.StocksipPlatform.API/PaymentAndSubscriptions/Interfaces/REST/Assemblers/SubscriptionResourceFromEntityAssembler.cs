﻿using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
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
    /// <param name="preferenceId">}
    ///     The preference id of the subscription in MercadoPago.
    /// </param>
    /// <param name="initPoint">
    ///     The url to redirect to after the subscription is created.
    /// </param>
    /// <returns>
    ///     A SubscriptionResource object.
    /// </returns>
    public static SubscriptionResource ToResourceFromEntity(string preferenceId, string initPoint)
    {
        return new SubscriptionResource(
            preferenceId,
            initPoint);
    }
}