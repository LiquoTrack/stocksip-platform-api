using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;


namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Application.Internal.CommandServices;

/// <summary>
/// Service implementation for handling purchase order commands.
/// </summary>
public class PurchaseOrderCommandService(IPurchaseOrderRepository purchaseOrderRepository) : IPurchaseOrderCommandService
{
    /// <summary>
    /// Handles the CreatePurchaseOrderCommand to create a new purchase order.
    /// </summary>
    /// <param name="command">The command containing the purchase order details.</param>
    /// <returns>The identifier of the created purchase order.</returns>
    public async Task<PurchaseOrderId> Handle(CreatePurchaseOrderCommand command)
    {
        var order = new PurchaseOrder(command);
        await purchaseOrderRepository.CreateAsync(order);
        return order.Id;
    }
    
    /// <summary>
    /// Handles the AddItemToOrderCommand to add an item to an existing purchase order.
    /// </summary>
    /// <param name="command">The command containing the item details.</param>
    /// <exception cref="InvalidOperationException">Thrown when the order is not found.</exception>
    public async Task Handle(AddItemToOrderCommand command)
    {
        var orderId = new PurchaseOrderId(command.orderId);
        var order = await purchaseOrderRepository.GetByIdAsync(orderId);

        if (order == null)
            throw new InvalidOperationException($"Order with ID {command.orderId} not found");

        order.AddItem(command);
        await purchaseOrderRepository.UpdateAsync(order);
    }

    /// <summary>
    /// Handles the RemoveItemFromOrderCommand to remove an item from a purchase order.
    /// </summary>
    /// <param name="command">The command containing the product identifier.</param>
    /// <exception cref="InvalidOperationException">Thrown when the order is not found.</exception>
    public async Task Handle(RemoveItemFromOrderCommand command)
    {
        var orderId = new PurchaseOrderId(command.orderId);
        var order = await purchaseOrderRepository.GetByIdAsync(orderId);

        if (order == null)
            throw new InvalidOperationException($"Order with ID {command.orderId} not found");

        order.RemoveItem(command);
        await purchaseOrderRepository.UpdateAsync(order);
    }

    /// <summary>
    /// Handles the ConfirmOrderCommand to confirm a purchase order.
    /// </summary>
    /// <param name="command">The command containing the order identifier.</param>
    /// <exception cref="InvalidOperationException">Thrown when the order is not found.</exception>
    public async Task Handle(ConfirmOrderCommand command)
    {
        var orderId = new PurchaseOrderId(command.orderId);
        var order = await purchaseOrderRepository.GetByIdAsync(orderId);

        if (order == null)
            throw new InvalidOperationException($"Order with ID {command.orderId} not found");

        order.ConfirmOrder();
        await purchaseOrderRepository.UpdateAsync(order);
    }

    /// <summary>
    /// Handles the ShipOrderCommand to mark an order as shipped.
    /// </summary>
    /// <param name="command">The command containing the order identifier.</param>
    /// <exception cref="InvalidOperationException">Thrown when the order is not found.</exception>
    public async Task Handle(ShipOrderCommand command)
    {
        var orderId = new PurchaseOrderId(command.orderId);
        var order = await purchaseOrderRepository.GetByIdAsync(orderId);

        if (order == null)
            throw new InvalidOperationException($"Order with ID {command.orderId} not found");

        order.ShipOrder();
        await purchaseOrderRepository.UpdateAsync(order);
    }

    /// <summary>
    /// Handles the ReceiveOrderCommand to mark an order as received.
    /// </summary>
    /// <param name="command">The command containing the order identifier.</param>
    /// <exception cref="InvalidOperationException">Thrown when the order is not found.</exception>
    public async Task Handle(ReceiveOrderCommand command)
    {
        var orderId = new PurchaseOrderId(command.orderId);
        var order = await purchaseOrderRepository.GetByIdAsync(orderId);

        if (order == null)
            throw new InvalidOperationException($"Order with ID {command.orderId} not found");

        order.ReceiveOrder();
        await purchaseOrderRepository.UpdateAsync(order);
    }

    /// <summary>
    /// Handles the CancelOrderCommand to cancel a purchase order.
    /// </summary>
    /// <param name="command">The command containing the order identifier.</param>
    /// <exception cref="InvalidOperationException">Thrown when the order is not found.</exception>
    public async Task Handle(CancelOrderCommand command)
    {
        var orderId = new PurchaseOrderId(command.orderId);
        var order = await purchaseOrderRepository.GetByIdAsync(orderId);

        if (order == null)
            throw new InvalidOperationException($"Order with ID {command.orderId} not found");

        order.CancelOrder();
        await purchaseOrderRepository.UpdateAsync(order);
    }
}