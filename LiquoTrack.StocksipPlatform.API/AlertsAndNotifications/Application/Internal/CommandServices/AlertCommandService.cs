using LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;

namespace LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Application.Internal.CommandServices
{
    /// <summary>
    /// This class implements the command service for handling alert-related commands.
    /// </summary>
    /// <param name="alertRepository">
    /// The repository for managing alerts.
    /// </param>
    /// <param name="unitOfWork">
    /// The unit of work for managing transactions.
    /// </param>
    public class AlertCommandService(IAlertRepository alertRepository,
    IUnitOfWork unitOfWork): IAlertCommandService
    {
        /// <summary>
        /// Handles the creation of a new alert.
        /// </summary>
        /// <param name="command">The command containing the alert details.</param>
        /// <returns>The created alert.</returns>
        public async Task<Alert?> Handle(CreateAlertCommand command)
        {
            var alert = new Alert(command);
            await alertRepository.AddAsync(alert);
            await unitOfWork.CompleteAsync();
            return alert;
        }
    }
}
