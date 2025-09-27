using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Entities;

/// <summary>
///     Class that represents a brand of a product in inventory.
/// </summary>
public class Brand(EBrandNames name) : Entity
{
    /// <summary>
    ///     The name of the brand.
    /// </summary>
    public EBrandNames Name { get; private set; } = name;
}