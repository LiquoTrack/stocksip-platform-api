using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;

/// <summary>
/// Query to get a care guide by its type of liquor and account ID.
/// </summary>
public record GetCareGuideByTypeOfLiquorQuery(
    string AccountId,
    EProductTypes TypeOfLiquor
);
