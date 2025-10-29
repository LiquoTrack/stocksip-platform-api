namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;

/// <summary>
///     Resource representing a collection of warehouses along with the total count.
/// </summary>
/// <param name="Total">
///     The total count of warehouses.
/// </param>
/// <param name="Warehouses">
///     A collection of warehouses.
/// </param>
public record WarehousesWithCountResource(int Total, List<WarehouseResource> Warehouses);