using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;

/// <summary>
///     Record representing a query to get the count of warehouses by AccountId.
/// </summary>
public record GetWarehousesCountByAccountIdQuery(AccountId AccountId);