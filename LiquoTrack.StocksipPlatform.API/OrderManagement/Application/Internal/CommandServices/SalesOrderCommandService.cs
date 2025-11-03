using System.ComponentModel.DataAnnotations;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using CatalogId = LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects.CatalogId;
using PurchaseOrderId = LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects.PurchaseOrderId;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Application.Internal.CommandServices
{
    /// <summary>
    /// Sales Order Command Service
    /// </summary>
    public class SalesOrderCommandService : ISalesOrderCommandService
    {
        private readonly ISalesOrderRepository salesOrderRepository;
        private readonly ILowStockService lowStockService;
        

        public SalesOrderCommandService(ISalesOrderRepository salesOrderRepository, ILowStockService lowStockService)
        {
            this.salesOrderRepository = salesOrderRepository ?? throw new ArgumentNullException(nameof(salesOrderRepository));
            this.lowStockService = lowStockService ?? throw new ArgumentNullException(nameof(lowStockService));
        }
        

        /// <summary>
        /// Handle the generate sales order command
        /// </summary>
        /// <param name="command"></param>
        /// <returns> The generated sales order </returns>
        public async Task<SalesOrder> Handle(GenerateSalesOrderCommand command)
        {
            if (command.items == null || !command.items.Any())throw new ValidationException("Cannot create an order with no items");

            var order = await salesOrderRepository.GenerateSalesOrder(command);
            order.UpdateStatus(ESalesOrderStatuses.Processing, "Order pending");
            
            await salesOrderRepository.UpdateAsync(order);
            return order;
        }

        /// <summary>
        /// Generate a purchase order (manual or automatic)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="accountId"></param>
        /// <returns> The generated sales order </returns>
        public async Task<SalesOrder> GeneratePurchaseOrder(GeneratePurchaseOrderRequest request, string accountId)
        {
            if (!request.IsAutomatic && (!request.Items.Any() || request.Items.All(i => i.QuantityToSell <= 0)))throw new ValidationException("Cannot create an order with no items");

            ICollection<SalesOrderItem> items;
            if (request.IsAutomatic)
            {
                items = await GetLowStockItems(accountId, request.CatalogToBuyFrom);
            }
            else
            {
                items = new List<SalesOrderItem>();
                foreach (var item in request.Items)
                {
                    var zeroMoney = new Money(0, new Currency(EValidCurrencyCodes.USD.ToString()));
                    var salesOrderItem = new SalesOrderItem(
                        new ProductId(item.ProductId),
                        zeroMoney,
                        item.QuantityToSell
                    );
                    salesOrderItem.InventoryId = string.IsNullOrEmpty(item.InventoryId) 
                        ? new LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects.InventoryId(Guid.NewGuid().ToString()) 
                        : new LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects.InventoryId(item.InventoryId!);
                    items.Add(salesOrderItem);
                }
            }

            if (!items.Any()) throw new InvalidOperationException("No items available for automatic order generation");

            var purchaseOrderId = new PurchaseOrderId(Guid.NewGuid().ToString());
            var command = new GenerateSalesOrderCommand(
                orderCode: request.OrderCode,
                purchaseOrderId: purchaseOrderId,
                items: items,
                status: ESalesOrderStatuses.Processing,
                catalogToBuyFrom: new CatalogId(request.CatalogToBuyFrom),
                receiptDate: request.ReceiptDate ?? DateTime.UtcNow.AddDays(7),
                completitionDate: request.CompletitionDate ?? DateTime.UtcNow.AddDays(14),
                accountId: new AccountId(accountId)
            );

            return await Handle(command);
        }

        private async Task<ICollection<SalesOrderItem>> GetLowStockItems(string accountId, string catalogId)
        {
            try
            {
                var lowStockItems = await lowStockService.GetLowStockItems(accountId, catalogId);

                var items = new List<SalesOrderItem>();

                foreach (var i in lowStockItems.Where(i => i != null && !string.IsNullOrWhiteSpace(i.ProductId) && i.SuggestedQuantity > 0))
                {
                    var zeroMoney = new Money(0, new Currency(EValidCurrencyCodes.USD.ToString()));
                    var salesOrderItem = new SalesOrderItem(
                        new ProductId(i.ProductId),
                        zeroMoney,
                        i.SuggestedQuantity
                    );
                    salesOrderItem.InventoryId = new InventoryId(Guid.NewGuid().ToString());
                    items.Add(salesOrderItem);
                }

                return items;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error while getting low stock items. Please try again later.", ex);
            }
        }

        /// <summary>
        /// Handle the update order status command
        /// </summary>
        public async Task<SalesOrder> Handle(UpdateOrderStatusCommand command)
        {
            try
            {
                var order = await salesOrderRepository.GetByIdAsync(command.OrderId) 
                    ?? throw new KeyNotFoundException($"Order with ID {command.OrderId} not found");

                if (order.Status == ESalesOrderStatuses.Canceled)
                {
                    throw new ValidationException("Canceled orders cannot be modified");
                }

                if (order.DeliveryProposal == null || order.DeliveryProposal.Status != DeliveryProposalStatus.Accepted)
                {
                    throw new ValidationException("State changes require prior consent from the Liquor Store Owner (accepted delivery proposal).");
                }

                order.UpdateStatus(command.NewStatus, command.Reason);
                await salesOrderRepository.UpdateAsync(order);
                
                return order;
            }
            catch (Exception ex) when (ex is not KeyNotFoundException && ex is not ValidationException)
            {
                throw;
            }
        }

        /// <summary>
        /// Supplier proposes a delivery schedule for an order
        /// </summary>
        public async Task<SalesOrder> Handle(ProposeDeliveryScheduleCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var order = await salesOrderRepository.GetByIdAsync(command.OrderId)
                        ?? throw new KeyNotFoundException($"Order with ID {command.OrderId} not found");
            order.ProposeDelivery(command.ProposedDate, command.Notes);
            await salesOrderRepository.UpdateAsync(order);
            return order;
        }

        /// <summary>
        /// Buyer responds to a delivery proposal
        /// </summary>
        /// <param name="command"></param>
        /// <returns> The updated sales order </returns>
        public async Task<SalesOrder> Handle(RespondDeliveryProposalCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var order = await salesOrderRepository.GetByIdAsync(command.OrderId)
                        ?? throw new KeyNotFoundException($"Order with ID {command.OrderId} not found");

            if (command.Accept)
                order.AcceptDeliveryProposal(command.Notes);
            else
                order.RejectDeliveryProposal(command.Notes);

            await salesOrderRepository.UpdateAsync(order);
            return order;
        }
    }
}
