using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries
{
    /// <summary>
    /// This query is used to retrieve all the care guides associated to a specific account id.
    /// </summary>
    /// <param name="AccountId">
    /// The unique identifier of the account that owns all the care guides that will be retrieved.
    /// </param>
    public record GetAllCareGuidesByAccountId(AccountId AccountId);
}
