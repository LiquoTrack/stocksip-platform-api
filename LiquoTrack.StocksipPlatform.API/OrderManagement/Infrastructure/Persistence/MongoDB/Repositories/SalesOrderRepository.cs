using Cortex.Mediator;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;
using MongoDB.Driver;
using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Infrastructure.Persistence.MongoDB.Repositories;

public class SalesOrderRepository : BaseRepository<SalesOrder>, ISalesOrderRepository
{
    private readonly IMongoCollection<SalesOrder> _salesOrders;

    public SalesOrderRepository(AppDbContext context, IMediator mediator) : base(context, mediator)
    {
        _salesOrders = context.GetCollection<SalesOrder>();
    }

    public async Task<SalesOrder> GenerateSalesOrder(GenerateSalesOrderCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        var salesOrder = new SalesOrder(
            command.orderCode,
            command.purchaseOrderId,
            command.items,
            command.status,
            command.catalogToBuyFrom,
            command.receiptDate,
            command.completitionDate,
            command.buyer);

        await AddAsync(salesOrder);
        await PublishEventsAsync(salesOrder);

        return salesOrder;
    }

    public async Task<IEnumerable<SalesOrder>> GetAllSalesOrdersByBuyerId(AccountId buyerId)
    {
        ArgumentNullException.ThrowIfNull(buyerId);

        var filter = Builders<SalesOrder>.Filter.Eq(x => x.Buyer, buyerId);
        return await _salesOrders.Find(filter).ToListAsync();
    }
}
