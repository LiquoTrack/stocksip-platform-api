using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.ACL.Services;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.External.ACL;

/// <summary>
///     Implementation of the <see cref="IPaymentAndSubscriptionsFacade"/> interface.
/// </summary>
/// <param name="accountCommandService">
///     The service for handling account-related commands.
/// </param>
/// <param name="accountQueryService">
///     The service for handling account-related queries.
/// </param>
/// <param name="businessCommandService">
///     The service for handling business-related commands.
/// </param>
public class PaymentAndSubscriptionsFacade(IAccountCommandService accountCommandService,
                                            IBusinessCommandService businessCommandService,
                                            ISubscriptionQueryService subscriptionQueryService,
                                            IAccountQueryService accountQueryService) 
                                            : IPaymentAndSubscriptionsFacade
{
    /// <summary>
    ///     Creates a new account.
    /// </summary>
    /// <param name="role">
    ///     The role of the account.
    /// </param>
    /// <param name="businessId">
    ///     The ID of the business associated with the account.
    /// </param>
    /// <returns>
    ///     The account object.
    /// </returns>
    public async Task<Account?> CreateAccount(string role, string businessId)
    {
        var command = new CreateAccountCommand(businessId, role);
        var account = await accountCommandService.Handle(command);
        return account;
    }

    /// <summary>
    ///     Creates a new business.
    /// </summary>
    /// <param name="businessName">
    ///     The name of the business. 
    /// </param>
    /// <returns>
    ///     The business object.
    /// </returns>
    public async Task<Business?> CreateBusiness(string businessName)
    {
        var command = new CreateBusinessCommand(businessName);
        var business = await businessCommandService.Handle(command);
        return business;
    }

    /// <summary>
    ///     Method to get the plan warehouse limit by account id.
    /// </summary>
    /// <param name="accountId">
    ///     The ID of the account.
    /// </param>
    /// <returns>
    ///     The warehouse limit for the plan associated with the account.
    /// </returns>
    public async Task<int?> GetPlanWarehouseLimitByAccountId(string accountId)
    {
        var query = new GetPlanWarehouseLimitByAccountId(accountId);
        var warehouseLimits = await subscriptionQueryService.Handle(query);
        return warehouseLimits;
    }

    /// <summary>
    ///     Method to get the plan products limit by account id.
    /// </summary>
    /// <param name="acconntId">
    ///     The ID of the account.         
    /// </param>
    /// <returns>
    ///     The product limit of the plan associated with the account.
    /// </returns>
    public async Task<int?> GetPlanProductsLimitByAccountId(string acconntId)
    {
        var query = new GetPlanProductsLimitByAccountIdQuery(acconntId);
        var productLimits = await subscriptionQueryService.Handle(query);
        return productLimits;
    }

    /// <summary>
    ///     Method to get the plan users limit by account id.
    /// </summary>
    /// <param name="accountId">
    ///     The ID of the account.       
    /// </param>
    /// <returns>
    ///     The user limit of the plan associated with the account.
    /// </returns>
    public async Task<int?> GetPlanUserLimitByAccountId(string accountId)
    {
        var query = new GetPlanUsersLimitByAccountIdQuery(accountId);
        var userLimits = await subscriptionQueryService.Handle(query);
        return userLimits;
    }
    
    /// <summary>
    ///     Gets all addresses associated with an account.
    /// </summary>
    /// <param name="accountId">
    ///     The account identifier.
    /// </param>
    /// <returns>
    ///     A collection of addresses.
    /// </returns>
    public async Task<IEnumerable<Address>> GetAccountAddressesAsync(string accountId)
    {
        var query = new GetAccountByIdQuery(accountId);
        var account = await accountQueryService.Handle(query);
        
        if (account == null)
            return Enumerable.Empty<Address>();
        
        return account.Addresses;
    }
    
    /// <summary>
    ///     Gets a specific address by account and address index.
    /// </summary>
    /// <param name="accountId">
    ///     The account identifier.
    /// </param>
    /// <param name="addressIndex">
    ///     The index of the address in the collection.
    /// </param>
    /// <returns>
    ///     The address if found, null otherwise.
    /// </returns>
    public async Task<Address?> GetAccountAddressAsync(string accountId, int addressIndex)
    {
        var addresses = await GetAccountAddressesAsync(accountId);
        var addressList = addresses.ToList();
        
        if (addressIndex < 0 || addressIndex >= addressList.Count)
            return null;
        
        return addressList[addressIndex];
    }
}