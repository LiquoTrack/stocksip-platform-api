using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Queries;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;

/// <summary>
///     Query service interface for retrieving plan information.
/// </summary>
public interface IPlansQueryService
{
    /// <summary>
    ///     Method to handle the retrieval of all available plans.
    /// </summary>
    /// <param name="query">
    ///     The query containing the parameters for retrieving all plans.
    /// </param>
    /// <returns>
    ///     A collection of all available plans.
    /// </returns>
    Task<IEnumerable<Plan>> Handle(GetAllPlansQuery query);
}