using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.QueryServices
{
    public class CareGuideQueryService(ICareGuideRepository careGuideRepository) : ICareGuideQueryService
    {
        public async Task<IEnumerable<CareGuide>> Handle(GetAllCareGuidesByAccountId query){
            return await careGuideRepository.GetAllByAccountId(query.AccountId.GetId);
        }
        public async Task<CareGuide?> Handle(GetCareGuideByIdQuery query) {
            return await careGuideRepository.GetById(query.Id);
        }
        public async Task<CareGuide?> Handle(GetCareGuideByProductIdQuery query) {
            var careGuides = await careGuideRepository.GetAllByProductId(query.ProductId);
            return careGuides.FirstOrDefault();
        }
        
        public async Task<CareGuide?> Handle(GetCareGuideByTypeOfLiquorQuery query) {
            return await careGuideRepository.GetByProductType(query.AccountId, query.TypeOfLiquor.ToString());
        }
    }
}
