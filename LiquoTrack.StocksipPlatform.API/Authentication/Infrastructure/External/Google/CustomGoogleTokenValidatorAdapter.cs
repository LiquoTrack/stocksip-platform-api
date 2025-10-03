using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google
{
    /// <summary>
    /// Adapter that implements IGoogleTokenValidator using the CustomGoogleTokenValidator
    /// </summary>
    public class CustomGoogleTokenValidatorAdapter : IGoogleTokenValidator
    {
        private readonly ISecurityTokenValidator _tokenValidator;
        private readonly ILogger<CustomGoogleTokenValidatorAdapter> _logger;

        public CustomGoogleTokenValidatorAdapter(ISecurityTokenValidator tokenValidator, ILogger<CustomGoogleTokenValidatorAdapter> logger)
        {
            _tokenValidator = tokenValidator;
            _logger = logger;
        }

        public async Task<GoogleTokenValidationResult> ValidateIdTokenAsync(string idToken)
        {
            try
            {
                _logger.LogInformation("Starting token validation...");
                _logger.LogDebug("Token: {Token}", idToken);
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuers = new[] { "https://accounts.google.com", "accounts.google.com" },
                    ValidateAudience = true,
                    ValidAudiences = new[] { 
                        "520776661353-aq0nbie37i8742tnn0167ak4bdadk2cu.apps.googleusercontent.com",
                        "520776661353-aq0nbie37i8742tnn0167ak4bdadk2cu.apps.googleusercontent.com"
                    },
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5),
                    RequireSignedTokens = true,
                    RequireExpirationTime = true,
                    SaveSigninToken = false,
                    ValidateActor = false,
                    ValidateTokenReplay = false
                };

                parameters.RoleClaimType = ClaimTypes.Role;
                parameters.NameClaimType = ClaimTypes.Name;
                parameters.ValidateIssuerSigningKey = false;
                parameters.SignatureValidator = (token, parameters) => new JwtSecurityToken(token);

                _logger.LogInformation("Validating token with parameters: {Parameters}", 
                    $"Issuers: {string.Join(", ", parameters.ValidIssuers ?? Array.Empty<string>())}, " +
                    $"Audiences: {string.Join(", ", parameters.ValidAudiences ?? Array.Empty<string>())}");

                var principal = _tokenValidator.ValidateToken(idToken, parameters, out var validatedToken);
                var jwtToken = validatedToken as JwtSecurityToken;

                _logger.LogInformation("Token validated successfully. Claims: {Claims}", 
                    string.Join(", ", jwtToken.Claims.Select(c => $"{c.Type}: {c.Value}")));

                if (jwtToken == null)
                {
                    return new GoogleTokenValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "Invalid JWT token"
                    };
                }

                var email = principal.FindFirst(ClaimTypes.Email)?.Value
                    ?? jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

                var name = principal.FindFirst(ClaimTypes.Name)?.Value
                    ?? jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Missing required claims. Available claims: {Claims}", 
                        string.Join(", ", jwtToken.Claims.Select(c => $"{c.Type}: {c.Value}")));

                    return new GoogleTokenValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = $"Missing required claims. Email: {!string.IsNullOrEmpty(email)}, Sub: {!string.IsNullOrEmpty(userId)}"
                    };
                }

                return new GoogleTokenValidationResult
                {
                    IsValid = true,
                    Email = email,
                    Name = name,
                    UserId = userId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Google token");
                return new GoogleTokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
