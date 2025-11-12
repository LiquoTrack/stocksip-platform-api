namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;

/// <summary>
///     Interface for managing ProductType entities.
/// </summary>
public interface ITypeRepository
{
    /// <summary>
    ///     Method to seed the product types in the database.
    /// </summary>
    /// <returns>
    ///     A confirmation of the seeding operation.
    /// </returns>
    Task SeedTypesNamesAsync();
}