using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;

/// <summary>
///     Record that represents a query to get all warehouses by account ID.
/// </summary>
public record GetAllWarehousesByAccountId(AccountId AccountId);