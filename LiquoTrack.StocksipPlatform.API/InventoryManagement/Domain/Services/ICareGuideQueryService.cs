using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services
{
    public interface ICareGuideQueryService
    {
        Task<IEnumerable<CareGuide>> Handle(GetAllCareGuidesByAccountId query);
        Task<CareGuide?> Handle(GetCareGuideByIdQuery query);
        Task<CareGuide?> Handle(GetCareGuideByProductIdQuery query);
        Task<CareGuide?> Handle(GetCareGuideByTypeOfLiquorQuery query);
    }
}
