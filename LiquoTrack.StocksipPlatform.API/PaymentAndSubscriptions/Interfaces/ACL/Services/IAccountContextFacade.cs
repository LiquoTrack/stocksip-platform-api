using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.ACL.Services;

public interface IAccountContextFacade
{
    Task<Account?> GetAccountByIdAsync(string accountId, Address address);
    
    Task<Account?> FindAccountByIdAsync(string accountId);
    Task<bool> AddAddressToAccountAsync(string accountId, Address address);
}