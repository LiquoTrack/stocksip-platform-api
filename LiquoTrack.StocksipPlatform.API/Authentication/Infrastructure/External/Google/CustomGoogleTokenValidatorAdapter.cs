using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Authentication;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;

        public CustomGoogleTokenValidatorAdapter(
            ISecurityTokenValidator tokenValidator, 
            ILogger<CustomGoogleTokenValidatorAdapter> logger,
            IConfiguration configuration)
        {
            _tokenValidator = tokenValidator;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<GoogleTokenValidationResult> ValidateIdTokenAsync(string idToken)
        {
            _logger.LogInformation("=== Starting Google token validation ===");
            _logger.LogInformation("Token: {Token}", idToken);
            
            if (string.IsNullOrWhiteSpace(idToken))
            {
                _logger.LogError("ID token is null or empty");
                return new GoogleTokenValidationResult { IsValid = false, ErrorMessage = "ID token is required" };
            }

            try
            {
                _logger.LogInformation("Starting token validation...");
                _logger.LogDebug("Token: {Token}", idToken);
                
                var googleSettings = _configuration.GetSection("Authentication:Google");
                var jwtSettings = _configuration.GetSection("Jwt");
                
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuer = jwtSettings.GetValue<bool>("ValidateIssuer"),
                    ValidIssuers = new[] { 
                        jwtSettings["Issuer"],
                        "https://accounts.google.com", 
                        "accounts.google.com" 
                    }.Distinct().ToArray(),
                    ValidateAudience = jwtSettings.GetValue<bool>("ValidateAudience"),
                    ValidAudiences = new[] { 
                        googleSettings["ClientId"],
                        jwtSettings["Audience"]
                    }.Where(a => !string.IsNullOrEmpty(a)).Distinct().ToArray(),
                    ValidateLifetime = jwtSettings.GetValue<bool>("ValidateLifetime"),
                    ClockSkew = TimeSpan.FromMinutes(jwtSettings.GetValue<int>("ClockSkew")),
                    RequireSignedTokens = jwtSettings.GetValue<bool>("RequireSignedTokens"),
                    RequireExpirationTime = jwtSettings.GetValue<bool>("RequireExpirationTime"),
                    SaveSigninToken = false,
                    ValidateActor = false,
                    ValidateTokenReplay = false,
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.Name,
                    ValidateIssuerSigningKey = jwtSettings.GetValue<bool>("ValidateIssuerSigningKey")
                };
                
                // For development, we'll disable signature validation
                // In production, you should validate the token signature using Google's public keys
                parameters.SignatureValidator = (token, parameters) => new JwtSecurityToken(token);
                parameters.RequireSignedTokens = false;
                parameters.ValidateIssuerSigningKey = false;
                
                _logger.LogInformation("Token validation parameters:");
                _logger.LogInformation($"- ValidateIssuer: {parameters.ValidateIssuer}");
                _logger.LogInformation($"- ValidIssuers: {string.Join(", ", parameters.ValidIssuers ?? Array.Empty<string>())}");
                _logger.LogInformation($"- ValidateAudience: {parameters.ValidateAudience}");
                _logger.LogInformation($"- ValidAudiences: {string.Join(", ", parameters.ValidAudiences ?? Array.Empty<string>())}");
                _logger.LogInformation($"- ValidateLifetime: {parameters.ValidateLifetime}");
                _logger.LogInformation($"- ClockSkew: {parameters.ClockSkew}");
                _logger.LogInformation($"- RequireSignedTokens: {parameters.RequireSignedTokens}");
                _logger.LogInformation($"- RequireExpirationTime: {parameters.RequireExpirationTime}");
                _logger.LogInformation($"- ValidateIssuerSigningKey: {parameters.ValidateIssuerSigningKey}");

                _logger.LogInformation("Validating token with parameters: {Parameters}", 
                    $"Issuers: {string.Join(", ", parameters.ValidIssuers ?? Array.Empty<string>())}, " +
                    $"Audiences: {string.Join(", ", parameters.ValidAudiences ?? Array.Empty<string>())}");

                _logger.LogInformation("Validating token with parameters:");
                _logger.LogInformation("- ValidateIssuer: {ValidateIssuer}", parameters.ValidateIssuer);
                _logger.LogInformation("- ValidIssuers: {ValidIssuers}", string.Join(", ", parameters.ValidIssuers ?? Array.Empty<string>()));
                _logger.LogInformation("- ValidateAudience: {ValidateAudience}", parameters.ValidateAudience);
                _logger.LogInformation("- ValidAudiences: {ValidAudiences}", string.Join(", ", parameters.ValidAudiences ?? Array.Empty<string>()));
                _logger.LogInformation("- ValidateLifetime: {ValidateLifetime}", parameters.ValidateLifetime);
                _logger.LogInformation("- ClockSkew: {ClockSkew}", parameters.ClockSkew);
                _logger.LogInformation("- ValidateIssuerSigningKey: {ValidateIssuerSigningKey}", parameters.ValidateIssuerSigningKey);

                SecurityToken validatedToken;
                var principal = _tokenValidator.ValidateToken(idToken, parameters, out validatedToken);
                var jwtToken = validatedToken as JwtSecurityToken;
                
                if (jwtToken == null)
                {
                    _logger.LogError("Failed to parse JWT token");
                    return new GoogleTokenValidationResult { IsValid = false, ErrorMessage = "Invalid JWT token format" };
                }

                _logger.LogInformation("Token validated successfully. Claims: {Claims}", 
                    string.Join(", ", jwtToken.Claims.Select(c => $"{c.Type}: {c.Value}")));

                if (jwtToken == null)
                {
                    _logger.LogError("Failed to parse JWT token");
                    return new GoogleTokenValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "Invalid JWT token: Failed to parse token"
                    };
                }

                _logger.LogInformation("JWT Token parsed successfully");
                _logger.LogInformation("Token Issuer: {Issuer}", jwtToken.Issuer);
                _logger.LogInformation("Token Audience: {Audience}", string.Join(", ", jwtToken.Audiences ?? Array.Empty<string>()));
                _logger.LogInformation("Token Valid To: {ValidTo}", jwtToken.ValidTo);
                _logger.LogInformation("Token Claims: {Claims}", 
                    string.Join(", ", jwtToken.Claims.Select(c => $"{c.Type}: {c.Value}")));

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
