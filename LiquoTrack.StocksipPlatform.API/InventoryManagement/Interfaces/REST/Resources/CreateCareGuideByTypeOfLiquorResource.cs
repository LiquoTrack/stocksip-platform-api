using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources
{
    /// <summary>
    /// This record represents a resource for creating a care guide by type of liquor.
    /// </summary>
    public record CreateCareGuideByTypeOfLiquorResource(
        EProductTypes TypeOfLiquor,
        string ProductName,
        string Title,
        string Summary,
        double RecommendedMinTemperature,
        double RecommendedMaxTemperature
    );
}
