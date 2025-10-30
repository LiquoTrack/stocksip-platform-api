﻿using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Queries;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;

/// <summary>
///     Query service interface for retrieving subscription information.
/// </summary>
public interface ISubscriptionQueryService
{
    /// <summary>
    ///     Method to handle the retrieval of a subscription by its preference ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the preference ID.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    Task<string?> Handle(GetSubscriptionStatusByPreferenceIdQuery query);

    /// <summary>
    ///     Method to handle the retrieval of a plan's warehouse limit by account ID.   '
    /// </summary>
    /// <param name="query">
    ///         The query object containing the account ID.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    Task<int?> Handle(GetPlanWarehouseLimitByAccountId query);
    
    /// <summary>
    ///     Method to handle the retrieval of a plan's products limit by account ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the account ID.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    Task<int?> Handle(GetPlanProductsLimitByAccountIdQuery query);

    /// <summary>
    ///     Method to handle the retrieval of a plan's users limit by account ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the account ID.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    Task<int?> Handle(GetPlanUsersLimitByAccountIdQuery query);
}