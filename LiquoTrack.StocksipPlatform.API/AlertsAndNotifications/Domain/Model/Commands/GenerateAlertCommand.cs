namespace LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Domain.Model.Commands;

public record GenerateAlertCommand(string AccountId,string Title, string Type, string Message);