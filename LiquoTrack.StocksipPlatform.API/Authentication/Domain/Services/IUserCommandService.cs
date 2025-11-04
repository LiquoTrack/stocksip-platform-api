using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services;

public interface IUserCommandService
{
    /// <summary>
    ///     Method to create a new user.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for creating a new user.
    /// </param>
    /// <returns>
    ///     A user object representing the newly created user.
    /// </returns>
    Task<User?> Handle(CreateUserCommand command);
    
    /// <summary>
    ///     Method to create or update a user from an external provider.
    /// </summary>
    /// <param name="providerUserId">
    ///     The ID of the user from the external provider. 
    /// </param>
    /// <param name="email">
    ///     The email address of the user.
    /// </param>
    /// <param name="name">
    ///     The name of the user.
    /// </param>
    /// <param name="accountId">
    ///     The ID of the account the user belongs to.
    /// </param>
    /// <returns>
    ///     A user object representing the created or updated user.
    /// </returns>
    Task<User> CreateOrUpdateFromExternalAsync(string providerUserId, string email, string? name, string? accountId = null);
    
    /// <summary>
    ///     Method to sign in a user.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for signing in a user.   
    /// </param>
    /// <returns>
    ///     A user object representing the signed in user.
    /// </returns>
    Task<(User user, string token)> Handle(SignInCommand command);
    
    /// <summary>
    ///     Method to register a new user.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for registering a new user.
    /// </param>
    /// <returns>
    ///     A user object representing the newly registered user.
    /// </returns>
    Task<User?> Handle(SignUpCommand command);

    /// <summary>
    ///     Method to register a sub user.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for registering a sub user.
    /// </param>
    /// <returns>
    ///     A user object representing the newly registered sub user.
    /// </returns>
    Task<User?> Handle(RegisterSubUserCommand command);

    Task<bool?> Handle(DeleteUserWithProfielByIdCommand command);
}