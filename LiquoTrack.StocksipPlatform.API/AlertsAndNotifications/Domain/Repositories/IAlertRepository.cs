using LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;

namespace LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Domain.Repositories;

public interface IAlertRepository: IBaseRepository<Alert>
{
    Task<List<Alert>> GetAllAlertsByAccountId(string accountId);
    Task<List<Alert>> GetAlertsByInventoryId(string inventoryId);
    Task<Alert> GenerateAlert(string accountId, string type, string message);
}