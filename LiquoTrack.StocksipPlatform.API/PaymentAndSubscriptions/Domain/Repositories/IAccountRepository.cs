using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Repositories;

/// <summary>
///     Repository interface for managing Account entities.
/// </summary>
public interface IAccountRepository : IBaseRepository<Account>
{
    /// <summary>
    ///     Method to get the status of an account by its ID.
    /// </summary>
    /// <param name="accountId">
    ///     The ID of the account whose status is to be retrieved.
    /// </param>
    /// <returns>
    ///     The status of the account.
    /// </returns>
    Task<string?> GetAccountStatusByIdAsync(string accountId);
}