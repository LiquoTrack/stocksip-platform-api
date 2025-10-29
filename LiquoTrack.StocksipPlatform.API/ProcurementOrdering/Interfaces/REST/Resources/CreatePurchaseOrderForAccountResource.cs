namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;

/// <summary>
/// Resource for creating a purchase order for a specific account.
/// </summary>
/// <param name="orderCode">The order code.</param>
/// <param name="catalogIdBuyFrom">The catalog ID to buy from.</param>
public record CreatePurchaseOrderForAccountResource(string orderCode, string catalogIdBuyFrom);