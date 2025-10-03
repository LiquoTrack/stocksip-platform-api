using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.ACL.Services;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.External.ACL;

/// <summary>
///     Implementation of the <see cref="IPaymentAndSubscriptionsFacade"/> interface.
/// </summary>
/// <param name="accountCommandService">
///     The service for handling account-related commands.
/// </param>
/// <param name="businessCommandService">
///     The service for handling business-related commands.
/// </param>
public class PaymentAndSubscriptionsFacade(IAccountCommandService accountCommandService,
                                            IBusinessCommandService businessCommandService) 
                                            : IPaymentAndSubscriptionsFacade
{
    /// <summary>
    ///     The service for handling account-related commands.
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
    ///     The service for handling business-related commands.
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
}