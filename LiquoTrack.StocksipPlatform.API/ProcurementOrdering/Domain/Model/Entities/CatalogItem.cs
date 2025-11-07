using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson.Serialization.Attributes;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Entities;

/// <summary>
/// Entity representing an item within a catalog.
/// </summary>
public class CatalogItem : Entity
{
    /// <summary>
    /// The product identifier.
    /// </summary>
    public ProductId ProductId { get; private set; }
    
    /// <summary>
    /// The product name.
    /// </summary>
    public string ProductName { get; private set; }
    
    /// <summary>
    /// The unit price of the product.
    /// </summary>
    public Money UnitPrice { get; private set; }
    
    /// <summary>
    /// The product image URL.
    /// </summary>
    public string ImageUrl { get; private set; }
    
    /// <summary>
    /// The date when the product was added to the catalog.
    /// </summary>
    public DateTime AddedAt { get; private set; }
    
    /// <summary>
    /// The available stock for this product in the associated warehouse.
    /// Can be null if stock tracking is not enabled for this catalog.
    /// </summary>
    public int? AvailableStock { get; private set; }

    /// <summary>
    /// Creates a new catalog item.
    /// </summary>
    /// <param name="productId">The product identifier.</param>
    /// <param name="productName">The product name.</param>
    /// <param name="unitPrice">The unit price.</param>
    /// <param name="imageUrl">The product image URL.</param>
    /// <param name="addedAt">The date when added to catalog.</param>
    /// <param name="availableStock">The available stock quantity (optional).</param>
    public CatalogItem(
        ProductId productId, 
        string productName, 
        Money unitPrice, 
        string imageUrl, 
        DateTime addedAt,
        int? availableStock = null)
    {
        ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
        ProductName = !string.IsNullOrWhiteSpace(productName)
            ? productName
            : throw new ArgumentException("Product name cannot be null or empty.", nameof(productName));
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
        ImageUrl = imageUrl;
        AddedAt = addedAt;
        
        if (availableStock.HasValue && availableStock.Value < 0)
            throw new ArgumentException("Available stock cannot be negative.", nameof(availableStock));
        
        AvailableStock = availableStock;
    }

    /// <summary>
    /// Updates the available stock for this catalog item.
    /// </summary>
    /// <param name="newStock">The new stock quantity.</param>
    /// <exception cref="ArgumentException">Thrown when the new stock is negative.</exception>
    public void UpdateStock(int newStock)
    {
        if (newStock < 0)
            throw new ArgumentException("Stock cannot be negative.", nameof(newStock));
        
        AvailableStock = newStock;
    }

    /// <summary>
    /// Checks if the item has sufficient stock for a given quantity.
    /// </summary>
    /// <param name="requestedQuantity">The requested quantity.</param>
    /// <returns>True if stock is sufficient or stock tracking is disabled, false otherwise.</returns>
    public bool HasSufficientStock(int requestedQuantity)
    {
        if (AvailableStock == null)
            return true;
        
        return AvailableStock >= requestedQuantity;
    }

    /// <summary>
    /// Calculates the subtotal for a given quantity of this item.
    /// </summary>
    /// <param name="quantity">The quantity to calculate.</param>
    /// <returns>The subtotal amount.</returns>
    public decimal CalculateSubTotal(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        
        return UnitPrice.GetAmount() * quantity;
    }

    // Protected constructor for MongoDB
    protected CatalogItem() { }
}