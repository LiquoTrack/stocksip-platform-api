using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Services;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Application.Internal.CommandServices
{
    /// <summary>
    /// Sales Order Command Service
    /// </summary>
    public class SalesOrderCommandService(ISalesOrderRepository salesOrderRepository) : ISalesOrderCommandService
    {
        /// <summary>
        /// Handle the generate sales order command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<SalesOrder> Handle(GenerateSalesOrderCommand command)
        {
            return await salesOrderRepository.GenerateSalesOrder(command);
        }
    }
}
