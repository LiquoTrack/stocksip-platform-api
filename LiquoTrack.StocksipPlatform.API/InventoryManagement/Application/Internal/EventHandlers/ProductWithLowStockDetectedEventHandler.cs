using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Events;
using LiquoTrack.StocksipPlatform.API.Shared.Application.Internal.EventHandlers;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.EventHandlers;

public class ProductWithLowStockDetectedEventHandler : IEventHandler<ProductWithLowStockDetectedEvent>
{
    public async Task Handle(ProductWithLowStockDetectedEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}