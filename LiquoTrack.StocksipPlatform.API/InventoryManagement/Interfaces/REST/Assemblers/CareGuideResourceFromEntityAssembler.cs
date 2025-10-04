using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers
{
    public static class CareGuideResourceFromEntityAssembler
    {
        public static CareGuideResource ToResourceFromEntity(CareGuide entity)
        {
            return new CareGuideResource(
                CareGuideId: entity.CareGuideId,
                AccountId: entity.AccountId.ToString(),
                ProductId: entity.ProductId,
                Title: entity.Title,
                Summary: entity.Summary,
                RecommendedMinTemperature: entity.RecommendedMinTemperature,
                RecommendedMaxTemperature: entity.RecommendedMaxTemperature,
                RecommendedPlaceStorage: entity.RecommendedPlaceStorage,
                GeneralRecommendation: entity.GeneralRecommendation
            );
        }
    }
}
