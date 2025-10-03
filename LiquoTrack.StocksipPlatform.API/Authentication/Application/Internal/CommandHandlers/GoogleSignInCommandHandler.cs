using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Authentication;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google.Responses;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.ACL.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.CommandHandlers
{
    public class GoogleSignInCommandHandler(IExternalAuthService _externalAuthService, IUserCommandService _userCommandService, IUserQueryService _userQueryService, IConfiguration _configuration, ILogger<GoogleSignInCommandHandler> _logger,IPaymentAndSubscriptionsFacade _paymentAndSubscriptionsFacade)
    {

        public async Task<(User? user, string? token, string? error)> Handle(GoogleSignInCommand command)
        {
            _logger.LogInformation("=== Starting Google Authentication ===");
            _logger.LogInformation($"Request received at: {DateTime.UtcNow:u}");

            if (string.IsNullOrWhiteSpace(command.IdToken))
            {
                _logger.LogWarning("Empty or null ID token provided");
                return (null, null, "ID token is required");
            }

            try
            {
                _logger.LogInformation("Validating Google ID token with external auth service...");
                var validationResult = await _externalAuthService.ValidateIdTokenAsync(command.IdToken);

                if (!validationResult.Success)
                {
                    _logger.LogWarning("Google token validation failed. Error: {Error}", validationResult.Error);
                    _logger.LogWarning("Validation result details: Success={Success}, Email={Email}, Name={Name}",
                        validationResult.Success, validationResult.Email, validationResult.Name);

                    return (null, null, validationResult.Error ?? "Unknown error during token validation");
                }

                _logger.LogInformation("Google token validation successful");
                _logger.LogInformation("User email from token: {Email}", validationResult.Email);
                _logger.LogInformation("User name from token: {Name}", validationResult.Name);

                var user = await GetOrCreateUserAsync(validationResult);
                if (user == null)
                {
                    return (null, null, "Failed to process user account");
                }

                var token = GenerateJwtToken(user);
                _logger.LogInformation("Authentication successful for user: {Email}", user.Email);

                return (user, token, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Google authentication");
                return (null, null, $"An error occurred during authentication: {ex.Message}");
            }
        }

        private async Task<User?> GetOrCreateUserAsync(ExternalAuthResult validationResult)
        {
            try
            {
                if (string.IsNullOrEmpty(validationResult.ProviderUserId) || string.IsNullOrEmpty(validationResult.Email))
                {
                    _logger.LogError("Invalid validation result - missing required fields");
                    return null;
                }

                var users = await _userQueryService.GetUsersByEmailAsync(new GetUserByEmailQuery(validationResult.Email));
                var user = users?.FirstOrDefault();

                if (user == null)
                {
                    _logger.LogInformation("Creating new user for external ID: {ExternalId}", validationResult.ProviderUserId);

                    user = await _userCommandService.CreateOrUpdateFromExternalAsync(
                        validationResult.ProviderUserId,
                        validationResult.Email,
                        validationResult.Name);

                    if (user == null)
                    {
                        _logger.LogError("Failed to create user for external ID: {ExternalId}", validationResult.ProviderUserId);
                        return null;
                    }

                    _logger.LogInformation("Created new user with ID: {UserId}", user.Id);
                }
                else
                {
                    _logger.LogDebug("Found existing user with ID: {UserId}", user.Id);
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting or creating user for external ID: {ExternalId}", validationResult.ProviderUserId);
                throw;
            }
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
