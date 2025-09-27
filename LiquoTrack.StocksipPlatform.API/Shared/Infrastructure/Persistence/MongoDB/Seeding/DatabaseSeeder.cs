using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;

namespace LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Seeding;

public class DatabaseSeeder(
    IBrandRepository brandRepository)
{
    /// <summary>
    ///     Method to seed the database with initial data.
    /// </summary>
    public async Task SeedAsync()
    {
        // Seed the brand names into the database
        await brandRepository.SeedBrandNames();
    }
}