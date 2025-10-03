using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Repositories;

/// <summary>
///     Interface for User repository operations
/// </summary>
public interface IUserRepository : IBaseRepository<User>
{
    /// <summary>
    ///     Finds a user by their email asynchronously
    /// </summary>
    /// <param name="email">The email to search for</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found, or null.</returns>
    Task<User?> FindByEmailAsync(string email);

    /// <summary>
    ///     Finds a user by their username asynchronously
    /// </summary>
    /// <param name="username">The username to search for</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found, or null.</returns>
    Task<User?> FindByUsernameAsync(string username);

    /// <summary>
    ///     Finds a user by either email or username asynchronously
    /// </summary>
    /// <param name="email">The email to search for</param>
    /// <param name="username">The username to search for</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found, or null.</returns>
    Task<User?> FindByEmailOrUsernameAsync(string email, string username);

    /// <summary>
    ///     Retrieves all users from the database.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of all user entities.</returns>
    Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Checks if a user with the specified username exists in the database.
    /// </summary>
    /// <param name="username">The username to check.</param>
    /// <returns>True if the user exists; otherwise, false.</returns>
    bool ExistsByUsername(string username);
    
}