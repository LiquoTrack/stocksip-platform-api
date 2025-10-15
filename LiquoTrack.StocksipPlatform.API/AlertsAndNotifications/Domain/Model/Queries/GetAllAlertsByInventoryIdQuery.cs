using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Domain.Model.Queries
{
    public record GetAllAlertsByInventoryIdQuery(InventoryId InventoryId);
}
