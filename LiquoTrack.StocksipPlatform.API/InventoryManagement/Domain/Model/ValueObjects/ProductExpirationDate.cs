using LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Persistence.MongoDB.Configuration.Serializers;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Exceptions;
using MongoDB.Bson.Serialization.Attributes;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;

/// <summary>
///     Represents the expiration date of a product in inventory.
/// </summary>
[BsonSerializer(typeof(ProductExpirationDateSerializer))]
public record ProductExpirationDate()
{
    /// <summary>
    ///     The value of the expiration date.
    /// </summary>
    private DateOnly Value { get; } = new DateOnly();

    /// <summary>
    ///     Default constructor for the ProductExpirationDate Value Object.
    /// </summary>
    /// <param name="date">
    ///     The expiration date.
    /// </param>
    /// <exception cref="ValueObjectValidationException">
    ///     Thrown when the provided expiration date is in the past.   
    /// </exception>
    public ProductExpirationDate(DateOnly date) : this()
    {
        if (!IsExpirationDateValid(date))
            throw new ValueObjectValidationException(nameof(date), "Expiration date must be in the future.");
        
        Value = date;
    }
    
    /// <summary>
    ///     Static method to validate if the expiration date is in the future. 
    /// </summary>
    /// <param name="date">
    ///     The expiration date to validate.
    /// </param>
    /// <returns>
    ///     True if the expiration date is in the future, false otherwise.
    /// </returns>
    private static bool IsExpirationDateValid(DateOnly date) => date > DateOnly.FromDateTime(DateTime.Now);

    /// <summary>
    ///     Retrieves the underlying expiration date value for the ProductExpirationDate Value Object.
    /// </summary>
    /// <returns>
    ///     The expiration date.
    /// </returns>
    public DateOnly GetValue() => Value;
}