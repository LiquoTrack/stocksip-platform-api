using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Entities;

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
    ///     The 
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
}