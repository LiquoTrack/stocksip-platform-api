namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Resources;

public record SalesOrderItemResource(
    string ProductId,
    decimal UnitPrice,
    string Currency,
    string? InventoryId,
    int QuantityToSell
);
