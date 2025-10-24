using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.OutBoundServices.PaymentProviders;
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

        var subscription = new Subscription(command.AccountId, command.SelectedPlanId);

        string preferenceId;
        string initPoint;

        switch (plan.PlanType)
        {
            case EPlanType.Free:
                subscription.ActivateFreePlan(plan);
                await subscriptionRepository.AddAsync(subscription);
                return (null, null);

            case EPlanType.Premium:
                var premiumPreference = mercadoPagoService.CreatePaymentPreference(
                    title: plan.Description,
                    price: plan.PlanPrice.GetAmount(),
                    currency: plan.PlanPrice.GetCurrencyCode(),
                    quantity: 1
                );
                preferenceId = premiumPreference.PreferenceId;
                initPoint = premiumPreference.InitPoint;
                break;
            
            case EPlanType.Enterprise:
                var preference = mercadoPagoService.CreatePaymentPreference(
                    title: plan.Description,
                    price: plan.PlanPrice.GetAmount(),
                    currency: plan.PlanPrice.GetCurrencyCode(),
                    quantity: 1
                );
                preferenceId = preference.PreferenceId;
                initPoint = preference.InitPoint;
                break;

            default:
                throw new InvalidOperationException("Unknown plan type.");
        }

        await subscriptionRepository.AddAsync(subscription);
        return (preferenceId, initPoint);
    }

    public Task<Subscription?> Handle(UpgradeSubscriptionCommand command)
    {
        throw new NotImplementedException();
    }

    public Task<Subscription?> Handle(ActivateTrialCommand command)
    {
        throw new NotImplementedException();
    }

    public Task<Subscription?> Handle(CancelSubscriptionCommand command)
    {
        throw new NotImplementedException();
    }

    public Task<Subscription?> Handle(ExpireSubscriptionCommand command)
    {
        throw new NotImplementedException();
    }
}