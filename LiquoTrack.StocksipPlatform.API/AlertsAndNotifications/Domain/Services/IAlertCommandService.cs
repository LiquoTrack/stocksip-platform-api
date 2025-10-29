using LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Domain.Model.Commands;

namespace LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Domain.Services;

public interface IAlertCommandService
{
    Task<Alert?> Handle(CreateAlertCommand command);
}