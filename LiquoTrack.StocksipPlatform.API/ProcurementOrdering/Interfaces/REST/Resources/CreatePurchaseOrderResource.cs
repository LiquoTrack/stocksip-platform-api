namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;

/// <summary>
/// Resource for creating a purchase order.
/// </summary>
/// <param name="orderCode">The order code.</param>
/// <param name="catalogIdBuyFrom">The catalog identifier to buy from.</param>
/// <param name="buyer">The buyer account identifier.</param>
public record CreatePurchaseOrderResource(
    string orderCode,
    string catalogIdBuyFrom,
    string buyer
);