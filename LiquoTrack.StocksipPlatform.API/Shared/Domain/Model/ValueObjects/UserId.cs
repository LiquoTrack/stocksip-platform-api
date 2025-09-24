using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Exceptions;

namespace LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

public record UserId()
{
    /// <summary>
    /// This is the value of the AccountId Value Object.
    /// </summary>
    private string Id { get; } = string.Empty;

    /// <summary>
    ///     Default constructor for the AccountId Value Object.
    ///     It is used to validate if the provided id is a non-empty string.
    /// </summary>
    /// <param name="id"> The identifier for the account </param>
    /// <exception cref="ArgumentException">Account ID must be a non-empty string.</exception>
    public UserId(string id) : this()
    {
        if (id == null || id.Trim().Length == 0)
        {
            throw new ValueObjectValidationException(nameof(UserId), "User ID must be a non-empty string.");
        }
        
        Id = id;
    }
    
    /// <summary>
    ///     This method returns the string representation of the AccountId Value Object.
    /// </summary>
    public string GetId => Id;
}