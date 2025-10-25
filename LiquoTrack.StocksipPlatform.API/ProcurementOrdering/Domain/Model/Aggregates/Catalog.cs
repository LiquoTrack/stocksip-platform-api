using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Aggregates;

/// <summary>
/// Aggregate entity representing a catalog.
/// </summary>
public class Catalog(
    string name,
    string description,
    AccountId ownerAccount,
    Email contactEmail
) : Entity
{
    /// <summary>
    /// The unique identifier of the catalog.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public CatalogId Id { get; private set; } = new(ObjectId.GenerateNewId().ToString());

    /// <summary>
    /// The name of the catalog.
    /// </summary>
    public string Name { get; private set; } = ValidateName(name);

    /// <summary>
    /// The description of the catalog.
    /// </summary>
    public string Description { get; private set; } = description;

    /// <summary>
    /// The list of items in the catalog.
    /// </summary>
    public List<CatalogItem> CatalogItems { get; private set; } = new();

    /// <summary>
    /// The account identifier of the owner.
    /// </summary>
    public AccountId OwnerAccount { get; private set; } = ownerAccount;

    /// <summary>
    /// The contact email for the catalog.
    /// </summary>
    public Email ContactEmail { get; private set; } = contactEmail;

    /// <summary>
    /// Indicates whether the catalog is published.
    /// </summary>
    public bool IsPublished { get; private set; } = false;

    /// <summary>
    /// Command constructor to create a new Catalog instance from a CreateCatalogCommand.
    /// </summary>
    /// <param name="command">
    /// The command containing the details to create a new catalog.
    /// </param>
    public Catalog(CreateCatalogCommand command)
        : this(command.name, command.description, new AccountId(command.ownerAccount), new Email(command.contactEmail))
    {
    }

    // Constructor for MongoDB deserialization
    public Catalog() : this(string.Empty, string.Empty, new AccountId(ObjectId.GenerateNewId().ToString()), new Email("default@example.com"))
    {
    }

    /// <summary>
    /// Validates the catalog name.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// The name cannot be null or empty.
    /// </exception>
    private static string ValidateName(string name)
        => string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("The catalog name cannot be null or empty.", nameof(name))
            : name;

    /// <summary>
    /// Publishes the catalog, making it visible to buyers.
    /// </summary>
    public void PublishCatalog()
    {
        IsPublished = true;
    }

    /// <summary>
    /// Unpublishes the catalog, making it invisible to buyers.
    /// </summary>
    public void UnpublishCatalog()
    {
        IsPublished = false;
    }

    /// <summary>
    /// Updates the catalog information using a command.
    /// </summary>
    /// <param name="command">
    /// The command containing the updated catalog information.
    /// </param>
    public void UpdateCatalog(UpdateCatalogCommand command)
    {
        Name = ValidateName(command.name);
        Description = command.description;
        ContactEmail = new Email(command.contactEmail);
    }

    /// <summary>
    /// Adds an item to the catalog using a command.
    /// </summary>
    /// <param name="command">
    /// The command containing the item details.
    /// </param>
    public void AddItem(AddItemToCatalogCommand command)
    {
        var money = new Money(command.amount, new Currency(command.currency));
        var item = new CatalogItem(command.productId, money, DateTime.UtcNow);
        CatalogItems.Add(item);
    }

    /// <summary>
    /// Removes an item from the catalog using a command.
    /// </summary>
    /// <param name="command">
    /// The command containing the product identifier to remove.
    /// </param>
    public void RemoveItem(RemoveItemFromCatalogCommand command)
    {
        var item = CatalogItems.FirstOrDefault(i => i.ProductId == command.productId);
        if (item != null)
            CatalogItems.Remove(item);
    }

    /// <summary>
    /// Gets the owner account identifier.
    /// </summary>
    /// <returns>
    /// The account identifier of the owner.
    /// </returns>
    public AccountId GetOwnerAccount() => OwnerAccount;
}