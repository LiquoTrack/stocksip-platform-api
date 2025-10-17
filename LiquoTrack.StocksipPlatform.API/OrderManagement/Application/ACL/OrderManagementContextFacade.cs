using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Application.ACL;

/// <summary>
/// Order Management Context Facade
/// </summary>
public class OrderManagementContextFacade(
    ISalesOrderCommandService salesOrderCommandService,
    ISalesOrderQueryService salesOrderQueryService)
    : IOrderManagementContextFacade
{
    /// <summary>
    /// Generate a sales order
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public async Task<SalesOrder?> GenerateSalesOrderAsync(GenerateSalesOrderCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        return await salesOrderCommandService.Handle(command);
    }

    /// <summary>
    /// Get all sales orders by buyer
    /// </summary>
    /// <param name="buyerId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<SalesOrder>> GetSalesOrdersByBuyerAsync(AccountId buyerId)
    {
        ArgumentNullException.ThrowIfNull(buyerId);
        var query = new GetAllSalesOrdersByBuyerIdQuery(buyerId);
        return await salesOrderQueryService.Handle(query);
    }
}
