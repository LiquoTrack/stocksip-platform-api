using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.ACL;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Application.ACL;

public class SalesOrderFacade : ISalesOrderFacade
{
    private readonly IPurchaseOrderRepository _orderRepository;

    public SalesOrderFacade(IPurchaseOrderRepository orderRepository)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }

    public async Task<string> ConvertPurchaseOrderToSalesOrderAsync(string purchaseOrderId)
    {
        var order = await _orderRepository.GetByIdAsync(new PurchaseOrderId(purchaseOrderId))
                    ?? throw new InvalidOperationException($"Purchase order {purchaseOrderId} not found.");
        
        var purchaseOrderResource = new PurchaseOrderResource(
            id: order.PurchaseOrderId.GetId,
            orderCode: order.OrderCode,
            items: order.Items.Select(i => new PurchaseOrderItemResource(
                productId: i.ProductId.GetId,
                quantity: i.Quantity,
                unitPrice: i.UnitPrice,
                subTotal: i.CalculateSubTotal()
            )).ToList(),
            status: order.Status.ToString(),
            catalogIdBuyFrom: order.CatalogIdBuyFrom.GetId(),
            generationDate: order.GenerationDate,
            confirmationDate: order.ConfirmationDate,
            buyer: order.Buyer.GetId,
            isOrderSent: order.IsOrderSent,
            total: order.Items.Sum(i => i.CalculateSubTotal())
        );
        
        var salesOrderId = await CreateSalesOrderFromPurchaseOrderAsync(purchaseOrderResource);
        return salesOrderId;
    }
    
    public Task<string> CreateSalesOrderFromPurchaseOrderAsync(PurchaseOrderResource purchaseOrder)
    {
        // Aquí solo mapeas o conviertes la orden de compra a un "SalesOrderCommand" o "SalesOrderResource"
        // y devuelves algún identificador o el objeto listo para que otro servicio lo cree

        // Por ejemplo, podrías devolver un ID provisional o simplemente el mismo PurchaseOrderId
        return Task.FromResult(purchaseOrder.id);

        // O si quieres devolver un SalesOrderCommand listo para crear la orden:
        // var command = new CreateSalesOrderCommand { ... map from purchaseOrder ... };
        // return Task.FromResult(command);
    }


}