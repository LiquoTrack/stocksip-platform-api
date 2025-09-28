using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;

/// <summary>
///     Record class that represents a command to update warehouse information.
/// </summary>
public record UpdateWarehouseInformationCommand(
    string Name, 
    WarehouseAddress NewAddress, 
    WarehouseTemperature NewTempLimits, 
    WarehouseCapacity TotalCapacity, 
    ImageUrl ImageUrl);