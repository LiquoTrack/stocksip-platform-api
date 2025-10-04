﻿using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers
{
    /// <summary>
    /// This class is responsible for transforming an UnassignCareGuideResource into an UnassignCareGuideCommand 
    /// </summary>
    public static class UnassignCareGuideCommandFromResourceAssembler
    {
        /// <summary>
        /// Transforms an UnassignCareGuideResource into an UnassignCareGuideCommand.
        /// </summary>
        /// <returns>
        /// The created UnassignCareGuideCommand.
        /// </returns>
        public static UnassignCareGuideCommand ToCommandFromResource(string careGuideId)
        {
            return new UnassignCareGuideCommand(careGuideId);
        }
    }
}
