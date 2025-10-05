using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration.Serializers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson.Serialization;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Persistence.MongoDB.Configuration.Serializers;

/// <summary>
///     Mapping configuration for the CareGuide entity.
/// </summary>
public class CareGuideMapping : IEntityTypeConfiguration<CareGuide>
{
    public void Configure(EntityTypeBuilder<CareGuide> builder){}

    public static void ConfigureBsonMapping()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(CareGuide))) return;
        
        BsonClassMap.RegisterClassMap<CareGuide>(map =>
        {
            map.AutoMap();
            
            map.MapMember(x => x.AccountId)
               .SetSerializer(new AccountIdSerializer())
               .SetElementName("accountId");
               
            map.MapMember(x => x.ProductId).SetElementName("productId");
            map.MapMember(x => x.Title).SetElementName("title");
            map.MapMember(x => x.Summary).SetElementName("summary");
            map.MapMember(x => x.RecommendedMinTemperature).SetElementName("recommendedMinTemperature");
            map.MapMember(x => x.RecommendedMaxTemperature).SetElementName("recommendedMaxTemperature");
            map.MapMember(x => x.RecommendedPlaceStorage).SetElementName("recommendedPlaceStorage");
            map.MapMember(x => x.GeneralRecommendation).SetElementName("generalRecommendation");
            
            map.UnmapMember(x => x.ProductAssociated);
        });
    }
}
