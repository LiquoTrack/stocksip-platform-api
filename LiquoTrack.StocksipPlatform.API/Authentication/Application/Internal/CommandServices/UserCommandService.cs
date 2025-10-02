using System.Security.Cryptography;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Hashing;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Token;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.ACL;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;
using RoleEntity = LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Entities.Role;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.CommandServices
{
    /// <summary>
    /// Command service for handling user-related operations.
    /// </summary>
    /// <remarks>
    /// This service is responsible for creating and updating user entities in the database.
    /// It also handles the creation of authentication tokens for the user.
    /// </remarks>
    public class UserCommandService : IUserCommandService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService? _tokenService;
        private readonly IHashingService _hashingService;

        public UserCommandService(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IHashingService hashingService,
            ITokenService? tokenService = null)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _hashingService = hashingService ?? throw new ArgumentNullException(nameof(hashingService));
            _tokenService = tokenService;
        }

        /// <summary>
        /// Handles the creation of a new user.
        /// </summary>
        /// <param name="command">The command containing user details.</param>
        /// <returns>The created user entity.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the command is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when a user with the same username already exists.</exception>
        public async Task<User?> Handle(CreateUserCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var existingUser = await _userRepository.FindByUsernameAsync(command.Username);
            if (existingUser != null)
                throw new InvalidOperationException($"Username {command.Username} is already taken");

            var hashedPassword = _hashingService.HashPassword(command.Password);
            var user = new User
            {
                Username = command.Username,
                Email = command.Email,
                Password = hashedPassword,
                UserRole = new RoleEntity { Name = EUserRoles.Normal },
            };

            try
            {
                await _userRepository.AddAsync(user);
                await _unitOfWork.CompleteAsync();
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while creating user: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates or updates a user from an external authentication provider.
        /// </summary>
        /// <param name="providerUserId">The unique identifier of the user from the external provider.</param>
        /// <param name="email">The email address of the user.</param>
        /// <param name="name">The name of the user (optional).</param>
        /// <param name="accountId">The account ID of the user (optional).</param>
        /// <returns>The created or updated user entity.</returns>
        /// <exception cref="ArgumentException">Thrown when the email is null or whitespace.</exception>
        public async Task<User> CreateOrUpdateFromExternalAsync(string providerUserId, string email, string? name = null, string? accountId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            var existingUser = await _userRepository.FindByUsernameAsync(email);

            if (existingUser != null)
            {
                return existingUser;
            }

            var randomPassword = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
            var hashedPassword = _hashingService.HashPassword(randomPassword);

            var user = new User
            {
                Username = !string.IsNullOrWhiteSpace(name) ? name : email.Split('@')[0],
                Email = new Email(email),
                Password = hashedPassword,
                UserRole = new RoleEntity { Name = EUserRoles.Normal }
            };

            try
            {
                await _userRepository.AddAsync(user);
                await _unitOfWork.CompleteAsync();
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while creating user from external provider: {ex.Message}", ex);
            }
        }

        /// <summary>
        ///     Handles the sign in command
        /// </summary>
        /// <param name="command">The sign in command</param>
        /// <returns>The user entity</returns>
        public async Task<User?> Handle(SignInCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var user = await _userRepository.FindByEmailAsync(command.Email);
            if (user == null)
                throw new InvalidOperationException($"User with email {command.Email} not found");

            var isPasswordValid = _hashingService.VerifyPassword(command.Password, user.Password);
            if (!isPasswordValid)
                throw new InvalidOperationException("Invalid password");

            return user;
        }

        /// <summary>
        ///     Handles the sign up command
        /// </summary>
        /// <param name="command">The sign up command</param>
        /// <returns>The user entity</returns>
        public async Task<User?> Handle(SignUpCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var existingUser = await _userRepository.FindByEmailAsync(command.Email);
            if (existingUser != null)
                throw new InvalidOperationException($"Email {command.Email} is already registered");

            var hashedPassword = _hashingService.HashPassword(command.Password);
            var user = new User
            {
                Username = command.Name,
                Email = new Email(command.Email),
                Password = hashedPassword,
                UserRole = new RoleEntity { Name = EUserRoles.Normal },
            };

            try
            {
                await _userRepository.AddAsync(user);
                await _unitOfWork.CompleteAsync();
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while creating user: {ex.Message}", ex);
            }
        }
    }
}