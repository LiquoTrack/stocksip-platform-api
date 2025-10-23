using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.CommandServices;

/// <summary>
///     Implementation of the <see cref="IAccountCommandService"/> interface for handling account-related commands.
/// </summary>
/// <param name="accountRepository">
///     The repository used to manage Account entities.
/// </param>
public class AccountCommandService(
    IAccountRepository accountRepository,
    IPlanRepository planRepository) : IAccountCommandService
{
    /// <summary>
    ///     The repository used to manage Account entities.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for creating a new account.
    /// </param>
    /// <returns>
    ///     A newly created <see cref="Account"/> instance, or null if the creation failed.
    /// </returns>
    public async Task<Account?> Handle(CreateAccountCommand command)
    {
        var account = new Account(command);
        await accountRepository.AddAsync(account);
        return account;
    }
}