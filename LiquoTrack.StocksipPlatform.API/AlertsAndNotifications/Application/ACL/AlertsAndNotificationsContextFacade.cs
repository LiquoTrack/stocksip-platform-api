using LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Domain.Services;
using LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Interfaces.ACL;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Application.ACL
{
    /// <summary>
    /// This class serves as a facade for the Alerts and Notifications context, providing methods to create alerts for other contexts.
    /// </summary>
    /// <param name="alertCommandService">
    /// The command service for handling alert operations.
    /// </param>
    /// <param name="alertQueryService">
    /// The query service for retrieving alert information.
    /// </param>
    public class AlertsAndNotificationsContextFacade(IAlertCommandService alertCommandService,
    IAlertQueryService alertQueryService) : IAlertsAndNotificationsContextFacade
    {
        /// <summary>
        /// Creates a new alert.
        /// </summary>
        /// <param name="title">The title of the alert.</param>
        /// <param name="message">The message of the alert.</param>
        /// <param name="severity">The severity of the alert.</param>
        /// <param name="type">The type of the alert.</param>
        /// <param name="inventoryId">The ID of the inventory associated with the alert.</param>
        /// <param name="profileId">The ID of the profile associated with the alert.</param>
        /// <returns>The ID of the created alert.</returns>
        public async Task<string> CreateAlert(string title, string message, string severity, string type, string inventoryId, string profileId)
        {
            if (string.IsNullOrEmpty(inventoryId))
                throw new ArgumentException("Inventory ID cannot be null or empty", nameof(inventoryId));
                
            var targetAccountId = new AccountId(profileId);
            var targetInventoryId = new InventoryId(inventoryId);
            var createAlertCommand = new CreateAlertCommand(title, message, severity, type, targetAccountId, targetInventoryId);
            var alert = await alertCommandService.Handle(createAlertCommand);
            return alert?.Id.ToString() ?? string.Empty;
        }
    }
}
