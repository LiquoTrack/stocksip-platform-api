using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.ACL.Services;

/// <summary>
///     Facade interface for handling account-related operations.
/// </summary>
public interface IPaymentAndSubscriptionsFacade
{
    /// <summary>
    ///     Method to create a new account.
    /// </summary>
    /// <param name="role">
    ///     The role of the account.
    /// </param>
    /// <param name="businessId">
    ///     The ID of the business associated with the account.
    /// </param>
    /// <returns>
    ///     An account object.
    /// </returns>
    Task<Account?> CreateAccount(string role, string businessId);
    
    /// <summary>
    ///     Method to create a new business.
    /// </summary>
    /// <param name="businessName">
    ///     The name of the business.   
    /// </param>
    /// <returns>
    ///     A business object.
    /// </returns>
    Task<Business?> CreateBusiness(string businessName);
    
    /// <summary>
    ///     Gets all addresses associated with an account.
    /// </summary>
    /// <param name="accountId">
    ///     The account identifier.
    /// </param>
    /// <returns>
    ///     A collection of addresses.
    /// </returns>
    Task<IEnumerable<Address>> GetAccountAddressesAsync(string accountId);
    
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
    Task<Address?> GetAccountAddressAsync(string accountId, int addressIndex);
}