using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;

/// <summary>
///     The repository for handling the Brands in the database.
/// </summary>
public interface IBrandRepository : IBaseRepository<Brand>
{
    /// <summary>
    ///     Method to seed the brand names in the database.
    /// </summary>
    /// <returns>
    ///     A confirmation of the seeding operation.
    /// </returns>
    Task SeedBrandNamesAsync();
}