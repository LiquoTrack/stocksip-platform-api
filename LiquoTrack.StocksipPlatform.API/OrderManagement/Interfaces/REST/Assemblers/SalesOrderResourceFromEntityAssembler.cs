using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Resources;
using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Assemblers
{
    public static class SalesOrderResourceFromEntityAssembler
    {
        public static SalesOrderResource ToResourceFromEntity(SalesOrder salesOrder)
        {
            ArgumentNullException.ThrowIfNull(salesOrder);

            return new SalesOrderResource(
                salesOrder.Id.ToString(),
                salesOrder.OrderCode,
                salesOrder.PurchaseOrderId.GetId,
                salesOrder.Items.Select(item => new SalesOrderItemResource(
                    item.ProductId.GetId,
                    item.UnitPrice.GetAmount(),
                    item.UnitPrice.GetCurrencyCode(),
                    item.InventoryId?.GetId,
                    item.QuantityToSell
                )),
                salesOrder.Status.ToString(),
                salesOrder.CatalogToBuyFrom.GetId(),
                salesOrder.ReceiptDate,
                salesOrder.CompletitionDate,
                salesOrder.Buyer.GetId
            );
        }
    }
}
