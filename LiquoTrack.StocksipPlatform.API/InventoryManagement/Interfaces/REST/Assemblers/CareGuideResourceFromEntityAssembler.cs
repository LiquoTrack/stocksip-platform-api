using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers
{
    public static class CareGuideResourceFromEntityAssembler
    {
        public static CareGuideResource ToResourceFromEntity(CareGuide entity)
        {
            var typeOfLiquor = entity.ProductAssociated?.Type.ToString();
            var productName = entity.ProductName ?? entity.ProductAssociated?.Name;
            var imageUrl = entity.ImageUrl ?? entity.ProductAssociated?.ImageUrl?.GetValue();

            return new CareGuideResource(
                CareGuideId: entity.CareGuideId,
                AccountId: entity.AccountId.GetId,
                ProductId: entity.ProductId,
                Title: entity.Title,
                Summary: entity.Summary,
                RecommendedMinTemperature: entity.RecommendedMinTemperature,
                RecommendedMaxTemperature: entity.RecommendedMaxTemperature,
                RecommendedPlaceStorage: entity.RecommendedPlaceStorage,
                GeneralRecommendation: entity.GeneralRecommendation,
                TypeOfLiquor: typeOfLiquor,
                ProductName: productName,
                ImageUrl: imageUrl
            );
        }
    }
}
