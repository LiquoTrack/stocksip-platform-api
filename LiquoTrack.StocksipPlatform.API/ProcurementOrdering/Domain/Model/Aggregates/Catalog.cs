using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson.Serialization.Attributes;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Aggregates;

/// <summary>
/// Aggregate entity representing a catalog.
/// </summary>
public class Catalog : Entity
{
    [BsonIgnore]
    public CatalogId CatalogId => new(Id.ToString());
    public string Name { get; private set; }
    public string Description { get; private set; }
    public List<CatalogItem> CatalogItems { get; private set; } = new();
    public AccountId OwnerAccount { get; private set; }
    public Email ContactEmail { get; private set; }
    public bool IsPublished { get; private set; } = false;

    public Catalog(string name, string description, AccountId ownerAccount, Email contactEmail)
    {
        Name = ValidateName(name);
        Description = description;
        OwnerAccount = ownerAccount;
        ContactEmail = contactEmail;
    }

    public Catalog(CreateCatalogCommand command)
        : this(command.name, command.description, new AccountId(command.ownerAccount), new Email(command.contactEmail))
    {
    }

    [BsonConstructor]
    protected Catalog() { }

    private static string ValidateName(string name)
        => string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("The catalog name cannot be null or empty.", nameof(name))
            : name;

    public void PublishCatalog() => IsPublished = true;
    public void UnpublishCatalog() => IsPublished = false;

    public void UpdateCatalog(UpdateCatalogCommand command)
    {
        Name = ValidateName(command.name);
        Description = command.description;
        ContactEmail = new Email(command.contactEmail);
    }

    /// <summary>
    /// Adds an item to the catalog using product data from another context.
    /// </summary>
    public void AddItem(string productId, string productName, decimal unitPrice, string currency, string imageUrl)
    {
        var money = new Money(unitPrice, new Currency(currency));
        var item = new CatalogItem(new ProductId(productId), productName, money, imageUrl, DateTime.UtcNow);
        CatalogItems.Add(item);
    }

    public void RemoveItem(RemoveItemFromCatalogCommand command)
    {
        var item = CatalogItems.FirstOrDefault(i => i.ProductId == command.productId);
        if (item != null)
            CatalogItems.Remove(item);
    }

    public AccountId GetOwnerAccount() => OwnerAccount;
}