using LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Domain.Model.Queries;

namespace LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Domain.Services
{
    public interface IAlertQueryService
    {
        Task<Alert?> Handle(GetAlertByIdQuery query);
    
        Task<IEnumerable<Alert>> Handle(GetAllAlertsByInventoryIdQuery query);
        Task<IEnumerable<Alert>> Handle(GetAllAlertsByAccountIdQuery query);
    }
}
