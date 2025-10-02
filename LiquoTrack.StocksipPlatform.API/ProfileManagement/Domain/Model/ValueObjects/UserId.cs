using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.ValueObjects;

/// <summary>
/// Value object representing a user identifier.
/// This is a shared value object used across bounded contexts.
/// </summary>
public record UserId
{
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Value { get; init; }

    /// <summary>
    /// Default constructor for MongoDB deserialization.
    /// </summary>
    public UserId()
    {
        Value = ObjectId.Empty;
    }

    /// <summary>
    /// Creates a new UserId from an ObjectId.
    /// </summary>
    /// <param name="value">The ObjectId value.</param>
    /// <exception cref="ArgumentException">Thrown when the ObjectId is empty.</exception>
    public UserId(ObjectId value)
    {
        if (value == ObjectId.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(value));

        Value = value;
    }

    /// <summary>
    /// Creates a new UserId from a string representation.
    /// </summary>
    /// <param name="value">The string representation of the ObjectId.</param>
    /// <exception cref="ArgumentException">Thrown when the string is invalid or empty.</exception>
    public UserId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("UserId string cannot be null or empty.", nameof(value));

        if (!ObjectId.TryParse(value, out var objectId))
            throw new ArgumentException("Invalid UserId format. Must be a valid ObjectId.", nameof(value));

        if (objectId == ObjectId.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(value));

        Value = objectId;
    }

    /// <summary>
    /// Generates a new unique UserId.
    /// </summary>
    /// <returns>A new UserId with a generated ObjectId.</returns>
    public static UserId GenerateNew()
    {
        return new UserId(ObjectId.GenerateNewId());
    }

    /// <summary>
    /// Tries to create a UserId from a string.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <param name="userId">The created UserId if successful.</param>
    /// <returns>True if the UserId was created successfully, false otherwise.</returns>
    public static bool TryCreate(string value, out UserId? userId)
    {
        userId = null;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        if (!ObjectId.TryParse(value, out var objectId))
            return false;

        if (objectId == ObjectId.Empty)
            return false;

        userId = new UserId(objectId);
        return true;
    }

    /// <summary>
    /// Converts the UserId to its string representation.
    /// </summary>
    /// <returns>The string representation of the ObjectId.</returns>
    public override string ToString() => Value.ToString();

    /// <summary>
    /// Implicit conversion from UserId to ObjectId.
    /// </summary>
    public static implicit operator ObjectId(UserId userId) => userId.Value;

    /// <summary>
    /// Implicit conversion from UserId to string.
    /// </summary>
    public static implicit operator string(UserId userId) => userId.Value.ToString();
}