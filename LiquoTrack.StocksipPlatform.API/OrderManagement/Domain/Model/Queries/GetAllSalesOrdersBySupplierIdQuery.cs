namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Queries;

using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

public record GetAllSalesOrdersBySupplierIdQuery(AccountId supplierId);
