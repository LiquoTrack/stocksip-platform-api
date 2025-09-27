using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;

/// <summary>
///     The Product Aggregate Root entity.
/// </summary>
public class Product(
    string name,
    EProductTypes type,
    string brand,
    Money unitPrice,
    ProductMinimumStock minimumStock,
    ImageUrl imageUrl,
    AccountId accountId,
    AccountId supplierId
    ) : Entity
{
    /// <summary>
    ///     The name of the product.
    /// </summary>
    public string Name { get; private set; } = name;

    /// <summary>
    ///     The type of the product.
    /// </summary>
    public EProductTypes Type { get; private set; } = type;

    /// <summary>
    ///     The brand of the product.
    /// </summary>
    public string Brand { get; private set; } = brand;

    /// <summary>
    ///     The unit price of the product.
    /// </summary>
    public Money UnitPrice { get; private set; } = unitPrice;

    /// <summary>
    ///     The minimum stock of the product before it is considered low of stock.
    /// </summary>
    private ProductMinimumStock MinimumStock { get; set; } = minimumStock;

    /// <summary>
    ///     The total stock in the store. Which sums up the stock of the product in all the warehouses.
    /// </summary>
    public int TotalStockInStore { get; private set; } = 0;
    
    /// <summary>
    ///     Indicates if the product is in the warehouse.
    /// </summary>
    public bool IsInWarehouse { get; private set; } = false;
    
    /// <summary>
    ///     The image url of the product.
    /// </summary>
    public ImageUrl ImageUrl { get; private set; } = imageUrl;

    /// <summary>
    ///     The identifier of the account that owns the product.
    /// </summary>
    public AccountId AccountId { get; private set; } = accountId;

    /// <summary>
    ///     The identifier of the supplier of the product. Can be null.
    /// </summary>
    public AccountId SupplierId { get; private set; } = supplierId;

    /// <summary>
    ///     Method to update the minimum stock of the product.
    /// </summary>
    /// <param name="minimumStock">
    ///     The new minimum stock value.
    /// </param>
    public void UpdateMinimumStock(int minimumStock) => MinimumStock.UpdateMinimumStock(minimumStock);

    /// <summary>
    ///     Method to update the total stock in store of the product.
    /// </summary>
    /// <param name="totalStockInStore">
    ///     The new total stock in store value.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     Thrown when the total stock in the store is negative.
    /// </exception>
    public void UpdateTotalStockInStore(int totalStockInStore)
    {
        if (totalStockInStore < 0)
            throw new ArgumentException("Total stock in store cannot be negative.", nameof(totalStockInStore));

        TotalStockInStore = totalStockInStore;
    }
    
    /// <summary>
    ///     Method to mark the product as being in the warehouse.
    /// </summary>
    public void StoreProduct() => IsInWarehouse = true;

    /// <summary>
    ///     Method to update the product information.
    /// </summary>
    /// <param name="name">
    ///     The new name of the product.    
    /// </param>
    /// <param name="unitPrice">
    ///     The new unit price of the product.
    /// </param>
    /// <param name="minimumStock">
    ///     The new minimum stock of the product.
    /// </param>
    /// <param name="imageUrl">
    ///     The new image url of the product.
    /// </param>
    public void UpdateInformation(
        string name,
        Money unitPrice,
        ProductMinimumStock minimumStock,
        ImageUrl imageUrl)
    {
        Name = name;
        UnitPrice = unitPrice;
        MinimumStock = minimumStock;
        ImageUrl = imageUrl;
    }
}