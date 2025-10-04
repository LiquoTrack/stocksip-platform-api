using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources
{
    /// <summary>
    /// This record represents a resource for creating a care guide by type of liquor.
    /// </summary>
    public record CreateCareGuideByTypeOfLiquorResource(
        /// <summary>
        /// The type of liquor for which to create a care guide.
        /// Valid values: Sodas, Snacks, Wines, Rums, Whiskeys, Beers, Tequilas, Vodkas, Gins, Cocktails, Juices, SoftDrinks, Others
        /// </summary>
        EProductTypes TypeOfLiquor
    );
}
