﻿using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Queries;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Services
{
    public interface ISalesOrderQueryService
    {
        Task<IEnumerable<SalesOrder>> Handle(GetAllSalesOrdersByBuyerIdQuery query);
    }
}
