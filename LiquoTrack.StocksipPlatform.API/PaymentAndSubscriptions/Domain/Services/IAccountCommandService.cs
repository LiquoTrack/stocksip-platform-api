using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Commands;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;

/// <summary>
///     Service interface for handling account-related commands.
/// </summary>
public interface IAccountCommandService
{
    /// <summary>
    ///     Method to handle the creation of a new account.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for creating a new account.
    /// </param>
    /// <returns>
    ///     The newly created account.
    ///     Or null if the account could not be created.
    /// </returns>
    Task<Account?> Handle(CreateAccountCommand command);
}