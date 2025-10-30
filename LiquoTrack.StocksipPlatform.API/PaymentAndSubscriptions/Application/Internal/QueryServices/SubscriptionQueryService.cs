﻿using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.QueryServices;

/// <summary>
///     Implementation of ISubscriptionQueryService for retrieving subscription information.
/// </summary>
public class SubscriptionQueryService(
    ISubscriptionRepository subscriptionRepository,
    IPlanRepository planRepository) : ISubscriptionQueryService
{
    /// <summary>
    ///     Method to handle the retrieval of a subscription by its preference ID. 
    /// </summary>
    /// <param name="query">
    ///     The query object containing the preference ID.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the subscription status.
    /// </returns>
    public async Task<string?> Handle(GetSubscriptionStatusByPreferenceIdQuery query)
    {
        return await subscriptionRepository.FindSubscriptionStatusByPreferenceId(query.PreferenceId);
    }

    /// <summary>
    ///     Method to handle the retrieval of a plan's warehouse limit by account ID. '
    /// </summary>
    /// <param name="query">
    ///     The query object containing the account ID.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the warehouse limit.
    /// </returns>
    public async Task<int?> Handle(GetPlanWarehouseLimitByAccountId query)
    {
        var subscription = await subscriptionRepository.FindActiveSubscriptionByAccountIdAsync(query.AccountId);
        if (subscription is null) return null;

        var warehouseLimit = await planRepository.FindPlanWarehouseLimitsByAccountIdAsync(subscription.PlanId);
        
        return warehouseLimit;
    }

    /// <summary>
    ///     Method to handle the retrieval of a plan's products limit by account ID.'
    /// </summary>
    /// <param name="query">
    ///     The query object containing the account ID.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the product limit.
    /// </returns>
    public async Task<int?> Handle(GetPlanProductsLimitByAccountIdQuery query)
    {
        var subscription = await subscriptionRepository.FindActiveSubscriptionByAccountIdAsync(query.AccountId);
        if (subscription is null) return null;
        
        var productsLimit = await planRepository.FindPlanProductsLimitByAccountIdAsync(subscription.PlanId);
        return productsLimit;
    }

    /// <summary>
    ///     Method to handle the retrieval of a plan's user limit by account ID.'
    /// </summary>
    /// <param name="query">
    ///     The query object containing the account ID.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the user limit.
    /// </returns>
    public async Task<int?> Handle(GetPlanUsersLimitByAccountIdQuery query)
    {
        var subscription = await subscriptionRepository.FindActiveSubscriptionByAccountIdAsync(query.accountId);
        if (subscription is null) return null;
        
        var userLimits = await planRepository.FindPlanUsersLimitByAccountIdAsync(subscription.PlanId);
        return userLimits;
    }
}