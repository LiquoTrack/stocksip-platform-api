using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;

/// <summary>
///     Query to retrieve all the product exit history for a specific account.
/// </summary>
public record GetAllProductExitsByAccountIdQuery(AccountId AccountId);