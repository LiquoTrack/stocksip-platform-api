using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Exceptions;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;

/// <summary>
///     Record class that serves as a Value Object for the content of a product.
/// </summary>
public record ProductContent()
{
    /// <summary>
    ///     The value of the product content.
    /// </summary>
    private double Value { get; } = 0;

    /// <summary>
    ///     Default constructor for ProductContent.
    /// </summary>
    /// <param name="content">
    ///     The content value of the product. Must be a non-negative value.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     Thrown when the provided content value is negative.
    /// </exception>
    public ProductContent(double content) : this()
    {
        if (!IsContentValid(content))
            throw new ValueObjectValidationException(nameof(content), "Product content must be a non-negative value.");
        
        Value = content;
    }
    
    /// <summary>
    ///     Static method to validate the content value.
    /// </summary>
    /// <param name="content">
    ///     The content value to validate.   
    /// </param>
    /// <returns>
    ///     True if the content value is a non-negative number, false otherwise. 
    /// </returns>
    private static bool IsContentValid(double content) => (content > 0);
    
    /// <summary>
    ///     Method to get the value of the product content.
    /// </summary>
    /// <returns>
    ///     The value of the product content. 
    /// </returns>
    public double GetValue() => Value;
}