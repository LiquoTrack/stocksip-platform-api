using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Commands;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Services;

public interface ISalesOrderCommandService
{
    Task<SalesOrder> Handle(GenerateSalesOrderCommand commnad);
    
}