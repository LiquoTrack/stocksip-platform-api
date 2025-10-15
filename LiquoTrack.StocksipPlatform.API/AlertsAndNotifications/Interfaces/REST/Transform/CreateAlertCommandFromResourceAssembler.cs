using LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Interfaces.REST.Transform
{
    /// <summary>
    /// This static class is responsible for transforming a CreateAlertResource into a CreateAlertCommand.
    /// </summary>
    public static class CreateAlertCommandFromResourceAssembler
    {
        public static CreateAlertCommand ToCommandFromResource(CreateAlertResource resource)
        {
            var accountId = new AccountId(resource.AccountId);
            var inventoryId = new InventoryId(resource.InventoryId);
            return new CreateAlertCommand(resource.Title, resource.Message, resource.Severity, resource.Type, accountId, inventoryId);
        }
    }
}
