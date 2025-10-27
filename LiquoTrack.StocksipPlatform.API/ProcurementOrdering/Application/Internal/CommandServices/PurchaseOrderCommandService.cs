using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Application.Internal.CommandServices;

/// <summary>
/// Application service responsible for handling all commands related to <see cref="PurchaseOrder"/>.
/// Implements business logic at the application layer and delegates persistence to repositories.
/// </summary>
public class PurchaseOrderCommandService : IPurchaseOrderCommandService
{
    private readonly IPurchaseOrderRepository _orderRepository;
    private readonly ICatalogRepository _catalogRepository;

    public PurchaseOrderCommandService(
        IPurchaseOrderRepository orderRepository,
        ICatalogRepository catalogRepository)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _catalogRepository = catalogRepository ?? throw new ArgumentNullException(nameof(catalogRepository));
    }

    public async Task<PurchaseOrderId> Handle(CreatePurchaseOrderCommand command)
    {
        var order = new PurchaseOrder(command);
        await _orderRepository.AddAsync(order);
        return order.PurchaseOrderId;
    }

    public async Task Handle(AddItemToOrderCommand command)
    {
        var order = await GetOrderByIdAsync(command.OrderId);

        var catalog = await _catalogRepository.GetByIdAsync(order.CatalogIdBuyFrom)
                      ?? throw new InvalidOperationException("Associated catalog not found.");

        var catalogItem = catalog.CatalogItems
                              .FirstOrDefault(i => i.ProductId.GetId.Trim().ToLower() == command.ProductId.Trim().ToLower())
                          ?? throw new InvalidOperationException($"Product with ID '{command.ProductId}' not found in catalog.");
        
        order.AddItem(catalogItem, command.Quantity);

        await _orderRepository.UpdateAsync(order);
    }

    public async Task Handle(RemoveItemFromOrderCommand command)
    {
        var order = await GetOrderByIdAsync(command.orderId);
        order.RemoveItem(command);
        await _orderRepository.UpdateAsync(order);
    }

    public async Task Handle(ConfirmOrderCommand command)
    {
        var order = await GetOrderByIdAsync(command.orderId);
        order.ConfirmOrder();
        await _orderRepository.UpdateAsync(order);
    }

    public async Task Handle(ShipOrderCommand command)
    {
        var order = await GetOrderByIdAsync(command.orderId);
        order.ShipOrder();
        await _orderRepository.UpdateAsync(order);
    }

    public async Task Handle(ReceiveOrderCommand command)
    {
        var order = await GetOrderByIdAsync(command.orderId);
        order.ReceiveOrder();
        await _orderRepository.UpdateAsync(order);
    }

    public async Task Handle(CancelOrderCommand command)
    {
        var order = await GetOrderByIdAsync(command.orderId);
        order.CancelOrder();
        await _orderRepository.UpdateAsync(order);
    }

    private async Task<PurchaseOrder> GetOrderByIdAsync(string orderId)
    {
        var id = new PurchaseOrderId(orderId);
        var order = await _orderRepository.GetByIdAsync(id);
        return order ?? throw new InvalidOperationException($"Order with ID '{orderId}' not found.");
    }
}