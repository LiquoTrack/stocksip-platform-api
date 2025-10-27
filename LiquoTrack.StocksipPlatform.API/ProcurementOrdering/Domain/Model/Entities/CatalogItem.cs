using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson.Serialization.Attributes;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Entities;

/// <summary>
/// Entity representing an item within a catalog.
/// </summary>
public class CatalogItem : Entity
{
    public ProductId ProductId { get; private set; }
    public string ProductName { get; private set; }
    public Money UnitPrice { get; private set; }
    public string ImageUrl { get; private set; }
    public DateTime AddedAt { get; private set; }

    public CatalogItem(ProductId productId, string productName, Money unitPrice, string imageUrl, DateTime addedAt)
    {
        ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
        ProductName = !string.IsNullOrWhiteSpace(productName)
            ? productName
            : throw new ArgumentException("Product name cannot be null or empty.", nameof(productName));
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
        ImageUrl = imageUrl;
        AddedAt = addedAt;
    }

    // Protected constructor for MongoDB
    protected CatalogItem() { }
}