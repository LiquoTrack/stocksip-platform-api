using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.QueryServices;

/// <summary>
///     Implementation of the <see cref="IWarehouseQueryService"/> interface.
/// </summary>
/// <param name="warehouseRepository">
///     The repository for handling the Warehouses in the database.
/// </param>
public class WarehouseQueryService(
        IWarehouseRepository warehouseRepository
    ) : IWarehouseQueryService
{
    /// <summary>
    ///     Method to get a warehouse by its ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the warehouse ID.
    /// </param>
    /// <returns>
    ///     The warehouse if found; otherwise, null.
    /// </returns>
    public async Task<Warehouse?> Handle(GetWarehouseByIdQuery query)
    {
        return await warehouseRepository.FindByIdAsync(query.WarehouseId);
    }

    /// <summary>
    ///     Method to get all warehouses associated with a specific account ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the account ID.
    /// </param>
    /// <returns>
    ///     A collection of warehouses associated with the account ID. Or an empty collection if none are found.
    /// </returns>
    public async Task<ICollection<Warehouse>> Handle(GetAllWarehousesByAccountId query)
    {
        return await warehouseRepository.FindByAccountIdAsync(query.AccountId);
    }

    /// <summary>
    ///     Method to get the count of warehouses associated with a specific account ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the account ID.
    /// </param>
    /// <returns>
    ///     The count of warehouses associated with the account ID. Or null if none are found.
    /// </returns>
    public async Task<int> Handle(GetWarehousesCountByAccountIdQuery query)
    {
        return await warehouseRepository.CountByAccountIdAsync(query.AccountId);
    }
}