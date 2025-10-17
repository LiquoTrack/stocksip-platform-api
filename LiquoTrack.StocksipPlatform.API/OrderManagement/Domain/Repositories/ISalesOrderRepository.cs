using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Repositories;

public interface ISalesOrderRepository : IBaseRepository<SalesOrder>
{
    Task<SalesOrder> GenerateSalesOrder(GenerateSalesOrderCommand command);
    Task<IEnumerable<SalesOrder>> GetAllSalesOrdersByBuyerId(AccountId buyerId);
}