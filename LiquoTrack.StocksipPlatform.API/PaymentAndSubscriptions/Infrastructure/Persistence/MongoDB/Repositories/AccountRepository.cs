using Cortex.Mediator;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;
using MongoDB.Driver;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Infrastructure.Persistence.MongoDB.Repositories;

/// <summary>
///     Repository implementation for Accounts entity.
/// </summary>
public class AccountRepository(AppDbContext context, IMediator mediator) : BaseRepository<Account> (context, mediator), IAccountRepository
{
    /// <summary>
    ///     The MongoDB collection for the Account entity.  
    /// </summary>
    private readonly IMongoCollection<Account> _accountCollection = context.GetCollection<Account>();
    
    /// <summary>
    ///     Method to get the status of an account by its ID. 
    /// </summary>
    /// <param name="accountId">
    ///     The ID of the account whose status is to be retrieved.
    /// </param>
    /// <returns>
    ///     The status of the account.
    /// </returns>
    public async Task<string?> GetAccountStatusByIdAsync(string accountId)
    {
        var account = await _accountCollection
            .Find(a => a.Id.ToString() == accountId)
            .FirstOrDefaultAsync();

        return account?.Status.ToString();
    }
}