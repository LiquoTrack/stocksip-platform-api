using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Entities;

/// <summary>
///     Class that represents a brand of a product in inventory.
/// </summary>
public class Brand : Entity
{
    /// <summary>
    ///     The name of the brand.
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    public EBrandNames Name { get; private set; }
    
    public Brand() { }
    
    public Brand(EBrandNames name)
    {
        Name = name;
    }
}