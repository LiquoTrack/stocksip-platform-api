using LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Interfaces.REST.Transform
{
    /// <summary>
    /// This class defines the assembler for converting an alert entity to a resource.
    /// </summary>
    public static class AlertResourceFromEntityAssembler
    {
        public static AlertResource ToResourceFromEntity(Alert entity)
        {
            return new AlertResource(
                entity.Id.ToString(),
                entity.Title,
                entity.Message,
                entity.Severity.ToString(),
                entity.Type.ToString(),
                entity.AccountId.GetId,
                entity.InventoryId.GetId
            );
        }
    }
}
