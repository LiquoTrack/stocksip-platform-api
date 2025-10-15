using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.QueryServices;

/// <summary>
///     Implementation of the <see cref="IPlanQueryService"/> interface.
/// </summary>
/// <param name="planRepository">
///     The repository for handling plan-related operations.
/// </param>
public class PlanQueryService(IPlanRepository planRepository) : IPlanQueryService
{
    /// <summary>
    ///     Method to handle the retrieval of all available plans.   
    /// </summary>
    /// <param name="query">
    ///     The query object containing parameters for retrieving all plans.
    /// </param>
    /// <returns>
    ///     The list of all available plans.
    /// </returns>
    public async Task<IEnumerable<Plan>> Handle(GetAllPlansQuery query)
    {
        return await planRepository.GetAllAsync();
    }
}