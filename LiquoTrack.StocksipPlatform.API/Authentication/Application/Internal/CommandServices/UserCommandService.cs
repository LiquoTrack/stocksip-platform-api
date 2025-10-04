using System.Security.Cryptography;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Hashing;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Token;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.ACL.Services;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.CommandServices
{
    /// <summary>
    /// Command service for handling user-related operations.
    /// </summary>
    /// <remarks>
    /// This service is responsible for creating and updating user entities in the database.
    /// It also handles the creation of authentication tokens for the user.
    /// </remarks>
    public class UserCommandService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IHashingService hashingService,
        IPaymentAndSubscriptionsFacade paymentAndSubscriptionsFacade
    ) : IUserCommandService
    {
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

            var existingUser = await userRepository.FindByUsernameAsync(command.Username);
            if (existingUser != null)
                throw new InvalidOperationException($"Username {command.Username} is already taken");

            var hashedPassword = hashingService.HashPassword(command.Password);
            var user = new User(
                new Email(command.Email.Value),
                command.Username,
                hashedPassword,
                "1234"
            );

            try
            {
                await userRepository.AddAsync(user);
                await unitOfWork.CompleteAsync();
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
        public async Task<User> CreateOrUpdateFromExternalAsync(string providerUserId, string email,
            string? name = null, string? accountId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            var existingUser = await userRepository.FindByUsernameAsync(email);

            if (existingUser != null)
            {
                return existingUser;
            }

            var randomPassword = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
            var hashedPassword = hashingService.HashPassword(randomPassword);

            var user = new User(
                new Email(email),
                string.IsNullOrWhiteSpace(name) ? email.Split('@')[0] : name,
                hashedPassword,
                "1234"
            );

            try
            {
                await userRepository.AddAsync(user);
                await unitOfWork.CompleteAsync();
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
        public async Task<(User user, string token)> Handle(SignInCommand command)
        {
            var user = await userRepository.FindByEmailAsync(command.Email);

            if (user == null || !hashingService.VerifyPassword(command.Password, user.Password))
                throw new Exception("Invalid username or password");

            var token = tokenService.GenerateToken(user);
            
            await unitOfWork.CompleteAsync();
            return (user, token);
        }

        /// <summary>
        ///     Handles the sign-up command
        /// </summary>
        /// <param name="command">The sign-up command</param>
        /// <returns>The user entity</returns>
        public async Task<User?> Handle(SignUpCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            try
            {
                var business = await paymentAndSubscriptionsFacade.CreateBusiness(
                    businessName: command.BusinessName
                );
                
                if (business == null) throw new Exception("Business creation failed");

                var account = await paymentAndSubscriptionsFacade.CreateAccount(
                    role: command.Role,
                    businessId: business.Id.ToString()
                );

                if (account == null) throw new Exception("Account creation failed");
                
                var existingUser = await userRepository.FindByEmailAsync(command.Email);
                if (existingUser != null)
                    throw new InvalidOperationException($"Email {command.Email} is already registered");

                var hashedPassword = hashingService.HashPassword(command.Password);
                var user = new User(
                    new Email(command.Email),
                    command.Name,
                    hashedPassword,
                    account.Id.ToString()
                );
                await userRepository.AddAsync(user);
                await unitOfWork.CompleteAsync();
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"SignUp failed: {ex.Message}", ex);
            }
        }
    }
}