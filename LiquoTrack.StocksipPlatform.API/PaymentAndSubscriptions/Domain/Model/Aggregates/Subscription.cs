using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;

/// <summary>
///     Aggregate root representing a subscription.
/// </summary>
public class Subscription(
    string accountId,
    string planId,
    ESubscriptionStatus status,
    DateTime expirationDate
    ) : Entity
{
    /// <summary>
    ///     The Account Id associated with the subscription.
    /// </summary>
    public string AccountId { get; set; } = accountId;
    
    /// <summary>
    ///     The Plan Id associated with the subscription.
    /// </summary>
    public string PlanId { get; set; } = planId;
    
    /// <summary>
    ///     The status of the subscription.
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    public ESubscriptionStatus Status { get; set; }
    
    /// <summary>
    ///     The expiration date of the subscription.
    /// </summary>
    public DateTime? ExpirationDate { get; set; } = expirationDate;

    /// <summary>
    ///     Method to activate a trial subscription for a given premium plan.
    /// </summary>
    /// <param name="premiumPLan">
    ///     The premium plan to activate the trial for.
    /// </param>
    public void ActivateTrial(Plan premiumPLan)
    {
        PlanId = premiumPLan.Id.ToString();
        Status = ESubscriptionStatus.Trial;
        ExpirationDate = DateTime.UtcNow.AddDays(14);
    }

    /// <summary>
    ///     Method to activate a paid subscription for a given paid plan.
    /// </summary>
    /// <param name="paidPlan">
    ///     The paid plan to activate the subscription for.
    /// </param>
    public void ActivatePaidPlan(Plan paidPlan)
    {
        PlanId = paidPlan.Id.ToString();
        Status = ESubscriptionStatus.Active;
        ExpirationDate = CalculateExpirationDate(paidPlan);
    }
    
    /// <summary>
    ///     Method to cancel the subscription.
    /// </summary>
    public void CancelSubscription()
    {
        Status = ESubscriptionStatus.Canceled;
        ExpirationDate = null;
    }
    
    /// <summary>
    ///     Method to mark the subscription as expired.
    /// </summary>
    public void MarkAsExpired()
    {
        Status = ESubscriptionStatus.Expired;
    }

    /// <summary>
    ///     Method to upgrade the subscription to a new plan.
    /// </summary>
    /// <param name="newPlan"></param>
    public void UpgradePlan(Plan newPlan)
    {
        PlanId = newPlan.Id.ToString();
        Status = ESubscriptionStatus.Active;
        ExpirationDate = CalculateExpirationDate(newPlan);
    }

    /// <summary>
    ///     Method to calculate the expiration date based on the plan's payment frequency.
    /// </summary>
    /// <param name="plan">
    ///     The plan to calculate the expiration date for.
    /// </param>
    /// <returns>
    ///     A <see cref="DateTime"/> representing the calculated expiration date.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     An exception thrown when the payment frequency is not recognized.
    /// </exception>
    public DateTime CalculateExpirationDate(Plan plan)
    {
        return plan.PaymentFrequency switch
        {
            EPaymentFrequency.Monthly => DateTime.UtcNow.AddMonths(1),
            EPaymentFrequency.Yearly => DateTime.UtcNow.AddYears(1),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}