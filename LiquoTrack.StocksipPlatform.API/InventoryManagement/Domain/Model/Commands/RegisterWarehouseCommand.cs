using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;

/// <summary>
///     Record representing the command to register a new warehouse.
/// </summary>
public record RegisterWarehouseCommand(
        string Name,
        WarehouseAddress Address,
        WarehouseTemperature Temperature,
        WarehouseCapacity Capacity,
        ImageUrl ImageUrl,
        AccountId AccountId
    );