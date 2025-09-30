using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Repositories;

/// <summary>
///     Repository interface for managing Plan entities.
/// </summary>
public interface IPlansRepository : IBaseRepository<Plan>
{
    /// <summary>
    ///     Method to seed predefined plans into the database.
    /// </summary>
    /// <returns>
    ///     A confirmation of the seeding operation.
    /// </returns>
    Task SeedPlansAsync();
}