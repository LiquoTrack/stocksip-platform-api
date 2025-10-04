﻿namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands
{
    /// <summary>
    /// Command to delete a care guide.
    /// </summary>
    public record DeleteCareGuideCommand(string careGuideId);
}
