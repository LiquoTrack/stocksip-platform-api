﻿namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;

/// <summary>
///     Command to delete a warehouse
/// </summary>
public record DeleteWarehouseCommand(string WarehouseId);