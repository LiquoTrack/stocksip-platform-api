using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers;

/// <summary>
///     Static class that provides methods for converting a <see cref="Warehouse"/> entity to a <see cref="WarehouseResource"/> resource.
/// </summary>
public static class WarehouseResourceFromEntityAssembler
{
    /// <summary>
    ///  Static method to convert Warehouse entity to WarehouseResource.
    /// </summary>
    /// <param name="entity">
    ///     The Warehouse entity to convert.   
    /// </param>
    /// <returns>
    ///     A WarehouseResource representation of the Warehouse entity. 
    /// </returns>
    public static WarehouseResource ToResourceFromEntity(Warehouse entity)
    {
        return new WarehouseResource
        (
            entity.Id.ToString(),
            entity.Name,
            entity.Address.Street,
            entity.Address.City,
            entity.Address.District,
            entity.Address.PostalCode,
            entity.Address.PostalCode,
            entity.Capacity.GetValue(),
            entity.Temperature.GetMinTemperature(),
            entity.Temperature.GetMaxTemperature(),
            entity.ImageUrl.GetValue()
        );
    }
}