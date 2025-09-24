using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Exceptions;

namespace LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

/// <summary>
///     The value object representing a currency.
/// </summary>
public record Currency()
{
    private readonly ICollection<string> _validCodes = ["USD", "EUR", "GBP", "JPY", "AUD", "CAD", "CHF", "CNY", "SEK", "PEN", "CHP"];
    
    /// <summary>
    ///     The currency code (e.g., "USD", "EUR").
    /// </summary>
    private string Code { get; } = string.Empty;
    
    /// <summary>
    ///     Default constructor for the Currency value object.
    /// </summary>
    /// <param name="code">
    ///     The currency code as a string.
    /// </param>
    /// <exception cref="ValueObjectValidationException">
    ///     Thrown when the provided currency code is not valid.
    /// </exception>
    public Currency(string code) : this()
    {
        if (!_validCodes.Contains(code))
        {
            throw new ValueObjectValidationException(
                nameof(Currency), 
                "Tried to use an invalid currency code. The valid codes are: " + string.Join(", ", _validCodes));
        }
        
        Code = code;
    }

    /// <summary>
    ///     Method to get the currency code.
    /// </summary>
    /// <returns>
    ///     The currency code as a string.
    /// </returns>
    public string GetCode() => Code;

    /// <summary>
    ///     Method to get the list of valid currency codes.
    /// </summary>
    /// <returns>
    ///     The collection of valid currency codes.
    /// </returns>
    public ICollection<string> GetValidCodes() => _validCodes;
    
    /// <summary>
    ///     Method to compare the currency code with another code and check if they are equal.
    /// </summary>
    /// <param name="otherCode">
    ///     The other currency code to compare with.
    /// </param>
    /// <returns>
    ///     True if the codes are equal and valid; otherwise, false.
    /// </returns>
    public bool AreCodesEqual(string otherCode) => _validCodes.Contains(otherCode) && Code == otherCode;
}