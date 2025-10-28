namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Resources;

public record CreateSalesOrderItemResource(
    string ProductId,
    decimal UnitPrice,
    string Currency,
    string? InventoryId,
    int QuantityToSell
);
