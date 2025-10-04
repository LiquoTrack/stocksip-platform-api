using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Assemblers;

/// <summary>
///     Static class to convert Account entity to AccountResource.
/// </summary>
public class AccountResourceFromEntityAssembler
{
    /// <summary>
    ///     Method to convert Account entity to AccountResource.  
    /// </summary>
    /// <param name="entity">
    ///     The Account entity to convert. 
    /// </param>
    /// <returns>
    ///     A new instance of AccountResource representing the provided entity.
    /// </returns>
    public static AccountResource ToResourceFromEntity(Account entity)
    {
        return new AccountResource(
            entity.Id.ToString(),
            entity.BusinessId,
            entity.Status.ToString(),
            entity.Role.ToString(),
            entity.GetCreationDate(),
            entity.Subscription.PlanId,
            entity.Subscription.Status.ToString(),
            entity.Subscription.ExpirationDate.ToString()
        );
    }
}