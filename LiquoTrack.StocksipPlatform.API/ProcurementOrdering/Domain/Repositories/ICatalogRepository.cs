using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Repositories;

/// <summary>
/// Repository interface for catalog operations.
/// </summary>
public interface ICatalogRepository
{
    /// <summary>
    /// Gets a catalog by its identifier.
    /// </summary>
    /// <param name="id">The catalog identifier.</param>
    /// <returns>The catalog if found, otherwise null.</returns>
    Task<Catalog?> GetByIdAsync(CatalogId id);

    /// <summary>
    /// Gets all catalogs.
    /// </summary>
    /// <returns>A collection of all catalogs.</returns>
    Task<IEnumerable<Catalog>> GetAllAsync();

    /// <summary>
    /// Gets all published catalogs.
    /// </summary>
    /// <returns>A collection of published catalogs.</returns>
    Task<IEnumerable<Catalog>> GetPublishedAsync();

    /// <summary>
    /// Gets all catalogs owned by a specific account.
    /// </summary>
    /// <param name="ownerAccount">The owner account identifier.</param>
    /// <returns>A collection of catalogs owned by the account.</returns>
    Task<IEnumerable<Catalog>> GetByOwnerAsync(AccountId ownerAccount);

    /// <summary>
    /// Creates a new catalog.
    /// </summary>
    /// <param name="catalog">The catalog to create.</param>
    /// <returns>The created catalog.</returns>
    Task<Catalog> CreateAsync(Catalog catalog);

    /// <summary>
    /// Updates an existing catalog.
    /// </summary>
    /// <param name="catalog">The catalog to update.</param>
    /// <returns>True if the update was successful, otherwise false.</returns>
    Task<bool> UpdateAsync(Catalog catalog);

    /// <summary>
    /// Deletes a catalog.
    /// </summary>
    /// <param name="id">The catalog identifier.</param>
    /// <returns>True if the deletion was successful, otherwise false.</returns>
    Task<bool> DeleteAsync(CatalogId id);
}