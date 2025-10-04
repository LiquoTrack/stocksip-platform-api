using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers
{
    /// <summary>
    /// This class is responsible for transforming a DeleteCareGuideResource into a DeleteCareGuideCommand. 
    /// </summary>
    public static class DeleteCareGuideCommandFromResourceAssembler
    {
        /// <summary>
        /// Transforms 
        /// </summary>
        /// <param name="careGuideId"></param>
        /// <returns></returns>
        public static DeleteCareGuideCommand ToCommandFromAssembler(string careGuideId)
        {
            return new DeleteCareGuideCommand(careGuideId);
        }
    }
}
