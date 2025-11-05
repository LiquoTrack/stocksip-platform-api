using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.OutBoundServices.PaymentProviders.services;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.CommandServices;

/// <summary>
///     Implementation of the <see cref="ISubscriptionsCommandService"/> interface.
/// </summary>
public class SubscriptionCommandService(
    ISubscriptionRepository subscriptionRepository,
    IAccountRepository accountRepository,
    IPlanRepository planRepository,
    IMercadoPagoService mercadoPagoService
    ) : ISubscriptionsCommandService
{
    /// <summary>
    ///     Method to handle the creation of a new subscription.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for creating a new subscription.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation. The task result contains the newly created subscription.
    /// </returns>
    /// <exception cref="Exception">
    ///     An exception is thrown if the account or plan could not be found.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     A thrown when an unknown plan type is encountered.
    /// </exception>
    public async Task<(string?, string?)> Handle(InitialSubscriptionCommand command)
    {
        var account = await accountRepository.FindByIdAsync(command.AccountId)
                      ?? throw new Exception($"Account with ID {command.AccountId} not found.");

        var plan = await planRepository.FindByIdAsync(command.SelectedPlanId)
                   ?? throw new Exception($"Plan with ID {command.SelectedPlanId} not found.");
        
        var pendingSubscription = await subscriptionRepository.FindPendingSubscriptionByAccountIdIdAsync(command.AccountId);
        var currentSubscription = await subscriptionRepository.FindActiveSubscriptionByAccountIdAsync(command.AccountId);
        
        if (pendingSubscription is not null) await subscriptionRepository.DeleteAsync(pendingSubscription.Id.ToString());
        if (currentSubscription is not null) throw new Exception("An active subscription already exists for this account.");
        
        var subscription = new Subscription(command.AccountId, command.SelectedPlanId);

        string? preferenceId = null;
        string? initPoint = null;

        switch (plan.PlanType)
        {
            case EPlanType.Free:
                subscription.ActivateFreePlan(plan);
                
                if (account.Status == EAccountStatuses.Inactive)
                {
                    account.ActivateAccount();
                    await accountRepository.UpdateAsync(account);
                }
                break;

            case EPlanType.Premium:
                var premiumPreference = mercadoPagoService.CreatePaymentPreference(
                    title: plan.Description,
                    price: plan.PlanPrice.GetAmount(),
                    currency: plan.PlanPrice.GetCurrencyCode(),
                    quantity: 1,
                    command.AccountId
                );
                preferenceId = premiumPreference.PreferenceId;
                initPoint = premiumPreference.InitPoint;
                subscription.MarkAsPending(plan, preferenceId);
                break;
            
            case EPlanType.Enterprise:
                var preference = mercadoPagoService.CreatePaymentPreference(
                    title: plan.Description,
                    price: plan.PlanPrice.GetAmount(),
                    currency: plan.PlanPrice.GetCurrencyCode(),
                    quantity: 1,
                    command.AccountId
                );
                preferenceId = preference.PreferenceId;
                initPoint = preference.InitPoint;
                subscription.MarkAsPending(plan, preferenceId);
                break;

            default:
                throw new InvalidOperationException("Unknown plan type.");
        }

        await subscriptionRepository.AddAsync(subscription);
        return (preferenceId, initPoint);
    }

    /// <summary>
    ///     Method to handle the confirmation of a payment for a subscription.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for confirming a payment.   
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation. The task result contains the updated subscription.
    /// </returns>
    /// <exception cref="Exception">
    ///     An exception is thrown if the subscription or plan could not be found.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     A thrown when an unknown payment status is encountered or when the plan type cannot be activated via payment confirmation.
    /// </exception>
    public async Task<Subscription?> Handle(ConfirmPaymentCommand command)
    {
        var subscription = await subscriptionRepository.FindByAccountIdAsync(command.AccountId)
                           ?? throw new Exception($"Subscription with Preference ID {command.AccountId} not found.");
        
        var plan = await planRepository.FindByIdAsync(subscription.PlanId)
                   ?? throw new Exception($"Plan with ID {subscription.PlanId} not found.");
        
        var account = await accountRepository.FindByIdAsync(subscription.AccountId)
                      ?? throw new Exception($"Account with ID {subscription.AccountId} not found.");

        switch (command.Status.ToLower())
        {
            case "approved":
                switch (plan.PlanType)
                {
                    case EPlanType.Premium:
                        subscription.ActivatePremiumPlan(plan);
                        break;

                    case EPlanType.Enterprise:
                        subscription.ActivateEnterprisePlan(plan);
                        break;

                    default:
                        throw new InvalidOperationException($"Plan type '{plan.PlanType}' cannot be activated via payment confirmation.");
                }

                if (account.Status == EAccountStatuses.Inactive)
                {
                    account.ActivateAccount();
                    await accountRepository.UpdateAsync(account);
                }
                break;
            default:
                throw new InvalidOperationException($"Unknown payment status: {command.Status}");
        }

        await subscriptionRepository.UpdateAsync(subscription);
        return subscription;
    }

    /// <summary>
    ///     Method to handle the webhook payment.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for handling the webhook payment. 
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation. The task result contains the updated subscription.
    /// </returns>
    /// <exception cref="Exception">
    ///     An exception is thrown if the payment could not be found or if the payment does not contain a valid Account ID.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     A thrown when the payment status is not approved.
    /// </exception>
    public async Task<Subscription?> Handle(WebhookPaymentCommand command)
    {
        var payment = await mercadoPagoService.GetPaymentById(command.paymentId)
                      ?? throw new Exception($"Payment with ID {command.paymentId} not found");

        if (payment.Status.ToLower() != "approved")
            throw new InvalidOperationException($"Payment status is '{payment.Status}', cannot confirm subscription");

        if (string.IsNullOrEmpty(payment.AccountId))
            throw new Exception("Payment does not contain a valid Account ID");

        var confirmPaymentCommand = new ConfirmPaymentCommand(payment.AccountId, payment.Status);
        return await Handle(confirmPaymentCommand);
    }


    public Task<Subscription?> Handle(UpgradeSubscriptionCommand command)
    {
        throw new NotImplementedException();
    }
}