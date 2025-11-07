using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.ACL.Services;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.External.ACL;

public class AccountContextFacade : IAccountContextFacade
{
    private readonly IMongoCollection<Account> _accounts;
    private readonly IMongoCollection<Business> _businesses;

    public AccountContextFacade(IMongoDatabase database)
    {
        _accounts = database.GetCollection<Account>("Accounts");
        _businesses = database.GetCollection<Business>("Businesses");
    }
    
    public async Task<Account?> GetAccountByIdAsync(string accountId, Address address)
    {
        throw new NotImplementedException();
    }
    
    public async Task<Account?> FindAccountByIdAsync(string accountId)
    {
        if (!ObjectId.TryParse(accountId, out var objectId))
            return null;

        var filter = Builders<Account>.Filter.Eq(a => a.Id, objectId);
        return await _accounts.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<bool> AddAddressToAccountAsync(string accountId, Address address)
    {
        if (!ObjectId.TryParse(accountId, out var objectId))
            return false;

        var filter = Builders<Account>.Filter.Eq(a => a.Id, objectId);
        var account = await _accounts.Find(filter).FirstOrDefaultAsync();
        
        if (account == null)
            return false;
        
        account.AddAddress(address);
        account.UpdatedAt = DateTime.UtcNow;
        
        var result = await _accounts.ReplaceOneAsync(filter, account);

        return result.ModifiedCount > 0;
    }
    
    public async Task<Business?> FindBusinessByAccountIdAsync(string accountId)
    {
        if (!ObjectId.TryParse(accountId, out var objectId))
            return null;

        var filter = Builders<Business>.Filter.Eq(b => b.Id, objectId);
        return await _businesses.Find(filter).FirstOrDefaultAsync();
    }

}
