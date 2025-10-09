using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers
{
    /// <summary>
    /// This class is responsible for transforming an AssignCareGuideToProductResource into an AssignCareGuideToProductCommand.
    /// </summary>
    public static class AssignCareGuideToProductCommandFromResource
    {
        /// <summary>
        /// Transforms an AssignCareGuideToProductResource into an AssignCareGuideToProductCommand.
        /// </summary>
        /// <returns>
        /// The transformed AssignCareGuideToProductCommand.
        /// </returns>
        public static AssignCareGuideToProductCommand ToCommandFromResource(string careGuideId, string productId) { 
            return new AssignCareGuideToProductCommand(
                careGuideId,
                productId
            );
        }
    }
}
