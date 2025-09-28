using System;
using System.Threading.Tasks;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.ACL;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.ACL;

/// <summary>
/// Facade for handling authentication-related operations by coordinating between command and query services.
/// </summary>
public class AuthenticationContextFacade : IAuthenticationContextFacade
{
    private readonly IUserCommandService _userCommandService;
    private readonly IUserQueryService _userQueryService;
    private readonly ILogger<AuthenticationContextFacade> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationContextFacade"/> class.
    /// </summary>
    /// <param name="userCommandService">The user command service.</param>
    /// <param name="userQueryService">The user query service.</param>
    /// <param name="logger">The logger instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the required services are null.</exception>
    public AuthenticationContextFacade(
        IUserCommandService userCommandService,
        IUserQueryService userQueryService,
        ILogger<AuthenticationContextFacade> logger)
    {
        _userCommandService = userCommandService ?? throw new ArgumentNullException(nameof(userCommandService));
        _userQueryService = userQueryService ?? throw new ArgumentNullException(nameof(userQueryService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates a new user with the specified username and password.
    /// </summary>
    /// <param name="username">The username of the new user.</param>
    /// <param name="password">The password for the new user.</param>
    /// <returns>The ID of the newly created user.</returns>
    /// <exception cref="ArgumentException">Thrown when username or password is null or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown when user creation fails.</exception>
    public async Task<string> CreateUserAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or whitespace.", nameof(username));
            
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or whitespace.", nameof(password));

        try
        {
            _logger.LogInformation("Initiating user creation for username: {Username}", username);
            
            var user = await _userCommandService.Handle(new CreateUserCommand(
                new Email($"{username}@example.com"), 
                password,
                username,
                new Role { Name = EUserRoles.Normal }
            ));
            
            if (user?.Id == ObjectId.Empty)
            {
                _logger.LogError("User creation failed for username: {Username}", username);
                throw new InvalidOperationException("User creation failed: Could not create user.");
            }

            _logger.LogInformation("Successfully created user with ID: {Id}", user.Id);
            return user.Id.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user with username: {Username}", username);
            throw new InvalidOperationException("An error occurred while creating the user.", ex);
        }
    }
}
