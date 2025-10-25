using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Entities;

public record CatalogItem
{
    public string ProductId { get; init; }
    public Money UnitPrice { get; init; }
    public DateTime AddedDate { get; init; }

    public CatalogItem(string productId, Money unitPrice, DateTime addedDate)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new ArgumentException("ProductId cannot be empty", nameof(productId));

        ProductId = productId;
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
        AddedDate = addedDate;
    }
}