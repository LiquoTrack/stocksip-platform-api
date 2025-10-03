using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;

/**
 * <summary>
 *     The user aggregate
 * </summary>
 * <remarks>
 *     This class is used to represent a user
 * </remarks>
 */
public class User : Entity
{
    public Email Email { get; set; }
    public string Username{ get; set; }
    public string Password { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public EUserRoles UserRole { get; set; }

    public User(Email email, string username, string hashedPassword)
    {
        Email = email;
        Username = username;
        Password = hashedPassword;
        UserRole = EUserRoles.Normal;
    }
    /**
     * <summary>
     *     Changes the password of the user
     * </summary>
     * <param name="hashedPassword">The hashed password</param>
     * <returns>The user</returns>
     */
    public User ChangePassword(string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new ArgumentException("The password cannot be empty.", nameof(hashedPassword));

        if (hashedPassword.Length < 8)
            throw new ArgumentException("The password must be at least 8 characters long.", nameof(hashedPassword));

        Password = hashedPassword;
        return this;
    }

    /**
     * <summary>
     *     Updates the username of the user
     * </summary>
     * <param name="newUsername">The new username</param>
     * <returns>The user</returns>
     */
    public User UpdateUsername(string newUsername)
    {
        if (string.IsNullOrWhiteSpace(newUsername))
            throw new ArgumentException("The username cannot be empty.", nameof(newUsername));

        if (newUsername.Length < 3)
            throw new ArgumentException("The username must be at least 3 characters.", nameof(newUsername));

        Username = newUsername.Trim();
        return this;
    }

    /**
     * <summary>
     *     Changes the role of the user
     * </summary>
     * <param name="newRoleId">The new role id</param>
     */
    public void ChangeRole(string newRoleId)
    {
        if (string.IsNullOrWhiteSpace(newRoleId))
            throw new ArgumentException("RoleId cannot be empty.", nameof(newRoleId));
        if (!Enum.TryParse<EUserRoles>(newRoleId, out var newRole));
    }
}