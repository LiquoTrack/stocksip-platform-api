using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Domain.Model.Commands
{
    public record CreateAlertCommand(
        string Title,
        string Message,
        string Severity,
        string Type,
        AccountId AccountId,
        InventoryId InventoryId);
}
