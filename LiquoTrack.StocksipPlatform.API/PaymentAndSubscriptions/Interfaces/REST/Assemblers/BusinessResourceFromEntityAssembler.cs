using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Assemblers;

/// <summary>
///     Static class to convert Business entity to BusinessResource.
/// </summary>
public class BusinessResourceFromEntityAssembler
{
    /// <summary>
    ///     Method to convert Business entity to BusinessResource.
    /// </summary>
    /// <param name="entity">
    ///     The Business entity to convert.
    /// </param>
    /// <returns>
    ///     A new instance of BusinessResource representing the provided entity.
    /// </returns>
    public static BusinessResource ToResourceFromEntity(Business entity)
    {
        return new BusinessResource(
            entity.BusinessName.Value,
            entity.BusinessEmail?.Value ?? string.Empty,
            entity.Ruc?.Value ?? string.Empty
        );
    }
}