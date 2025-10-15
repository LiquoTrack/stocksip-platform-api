using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories
{
    /// <summary>
    /// This interface defines the contract for a repository that manages Care Guide aggregates.
    /// </summary>
    public interface ICareGuideRepository : IBaseRepository<CareGuide>
    {
        Task<CareGuide?> GetById(string id);
        Task<IEnumerable<CareGuide>> GetAllByAccountId(string accountId);
        Task<IEnumerable<CareGuide>> GetAllByProductId(string productId);
        Task<CareGuide?> GetByProductType(string accountId, string productType);
        Task UpdateByCareGuideIdAsync(string careGuideId, CareGuide entity);
    }
}
