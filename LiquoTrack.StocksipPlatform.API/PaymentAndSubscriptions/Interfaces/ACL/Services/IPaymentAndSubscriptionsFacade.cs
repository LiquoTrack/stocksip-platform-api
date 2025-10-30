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
    ///     Method to get the maximum number of warehouses allowed for an account.
    /// </summary>
    /// <param name="accountId">
    ///     The ID of the account. 
    /// </param>
    /// <returns>
    ///     The maximum number of warehouses allowed for the account.
    /// </returns>
    Task<int?> GetPlanWarehouseLimitByAccountId(string accountId);
}