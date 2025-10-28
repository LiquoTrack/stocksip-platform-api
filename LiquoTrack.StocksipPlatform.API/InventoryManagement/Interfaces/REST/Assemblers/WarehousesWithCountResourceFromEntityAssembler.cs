using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers;

public class WarehousesWithCountResourceFromEntityAssembler
{
    public static WarehousesWithCountResource ToResourceFromEntity(ICollection<Warehouse> warehouses, int total)
    {
        var warehouseResource = warehouses.Select(WarehouseResourceFromEntityAssembler
                .ToResourceFromEntity)
                .ToList();
        return new WarehousesWithCountResource(total, warehouseResource);
    }
}