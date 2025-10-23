using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Commands;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;

/// <summary>
///     Command service interface for handling subscription-related commands.
/// </summary>
public interface ISubscriptionsCommandService
{
    /// <summary>
    ///     Method to handle the activation of a paid subscription.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for activating a paid subscription.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the activated subscription.
    /// </returns>
    Task<(Subscription?, string?)> Handle(InitialSubscriptionCommand command);
    
    /// <summary>
    ///     Method to handle the upgrade of a subscription.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for upgrading a subscription.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the upgraded subscription.
    /// </returns>
    Task<Subscription?> Handle(UpgradeSubscriptionCommand command);
    
    /// <summary>
    ///     Method to handle the activation of a trial subscription.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for activating a trial subscription.
    /// </param>
    /// <returns>
    ///      A task that represents the asynchronous operation. The task result contains the activated trial subscription.
    /// </returns>
    Task<Subscription?> Handle(ActivateTrialCommand command);
    
    /// <summary>
    ///     Method to handle the cancellation of a subscription.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for canceling a subscription.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the canceled subscription.
    /// </returns>
    Task<Subscription?> Handle(CancelSubscriptionCommand command);
    
    /// <summary>
    ///     Method to handle the expiration of a subscription.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for expiring a subscription.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the expired subscription.
    /// </returns>
    Task<Subscription?> Handle(ExpireSubscriptionCommand command);
}