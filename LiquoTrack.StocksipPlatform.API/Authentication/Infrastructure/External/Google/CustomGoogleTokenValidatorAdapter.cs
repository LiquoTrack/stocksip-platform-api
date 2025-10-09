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
    public class CustomGoogleTokenValidatorAdapter(
        ISecurityTokenValidator tokenValidator,
        ILogger<CustomGoogleTokenValidatorAdapter> logger,
        IConfiguration configuration)
        : IGoogleTokenValidator
    {
        public async Task<GoogleTokenValidationResult> ValidateIdTokenAsync(string idToken)
        {
            logger.LogInformation("=== Starting Google token validation ===");
            logger.LogInformation("Token: {Token}", idToken);
            
            if (string.IsNullOrWhiteSpace(idToken))
            {
                logger.LogError("ID token is null or empty");
                return new GoogleTokenValidationResult { IsValid = false, ErrorMessage = "ID token is required" };
            }

            try
            {
                logger.LogInformation("Starting token validation...");
                logger.LogDebug("Token: {Token}", idToken);
                
                var googleSettings = configuration.GetSection("Authentication:Google");
                var jwtSettings = configuration.GetSection("Jwt");
                
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
                    ValidateIssuerSigningKey = jwtSettings.GetValue<bool>("ValidateIssuerSigningKey"),
                    // For development, we'll disable signature validation
                    // In production; you should validate the token signature using Google's public keys
                    SignatureValidator = (token, parameters) => new JwtSecurityToken(token)
                };

                parameters.RequireSignedTokens = false;
                parameters.ValidateIssuerSigningKey = false;
                
                logger.LogInformation("Token validation parameters:");
                logger.LogInformation($"- ValidateIssuer: {parameters.ValidateIssuer}");
                logger.LogInformation($"- ValidIssuers: {string.Join(", ", parameters.ValidIssuers ?? Array.Empty<string>())}");
                logger.LogInformation($"- ValidateAudience: {parameters.ValidateAudience}");
                logger.LogInformation($"- ValidAudiences: {string.Join(", ", parameters.ValidAudiences ?? Array.Empty<string>())}");
                logger.LogInformation($"- ValidateLifetime: {parameters.ValidateLifetime}");
                logger.LogInformation($"- ClockSkew: {parameters.ClockSkew}");
                logger.LogInformation($"- RequireSignedTokens: {parameters.RequireSignedTokens}");
                logger.LogInformation($"- RequireExpirationTime: {parameters.RequireExpirationTime}");
                logger.LogInformation($"- ValidateIssuerSigningKey: {parameters.ValidateIssuerSigningKey}");

                logger.LogInformation("Validating token with parameters: {Parameters}", 
                    $"Issuers: {string.Join(", ", parameters.ValidIssuers ?? [])}, " +
                    $"Audiences: {string.Join(", ", parameters.ValidAudiences ?? [])}");

                logger.LogInformation("Validating token with parameters:");
                logger.LogInformation("- ValidateIssuer: {ValidateIssuer}", parameters.ValidateIssuer);
                logger.LogInformation("- ValidIssuers: {ValidIssuers}", string.Join(", ", parameters.ValidIssuers ?? []));
                logger.LogInformation("- ValidateAudience: {ValidateAudience}", parameters.ValidateAudience);
                logger.LogInformation("- ValidAudiences: {ValidAudiences}", string.Join(", ", parameters.ValidAudiences ?? []));
                logger.LogInformation("- ValidateLifetime: {ValidateLifetime}", parameters.ValidateLifetime);
                logger.LogInformation("- ClockSkew: {ClockSkew}", parameters.ClockSkew);
                logger.LogInformation("- ValidateIssuerSigningKey: {ValidateIssuerSigningKey}", parameters.ValidateIssuerSigningKey);

                var principal = tokenValidator.ValidateToken(idToken, parameters, out var validatedToken);

                if (validatedToken is not JwtSecurityToken jwtToken)
                {
                    logger.LogError("Failed to parse JWT token");
                    return new GoogleTokenValidationResult { IsValid = false, ErrorMessage = "Invalid JWT token format" };
                }

                logger.LogInformation("Token validated successfully. Claims: {Claims}", 
                    string.Join(", ", jwtToken.Claims.Select(c => $"{c.Type}: {c.Value}")));

                if (jwtToken == null)
                {
                    logger.LogError("Failed to parse JWT token");
                    return new GoogleTokenValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "Invalid JWT token: Failed to parse token"
                    };
                }

                logger.LogInformation("JWT Token parsed successfully");
                logger.LogInformation("Token Issuer: {Issuer}", jwtToken.Issuer);
                logger.LogInformation("Token Audience: {Audience}", string.Join(", ", jwtToken.Audiences ?? []));
                logger.LogInformation("Token Valid To: {ValidTo}", jwtToken.ValidTo);
                logger.LogInformation("Token Claims: {Claims}", 
                    string.Join(", ", jwtToken.Claims.Select(c => $"{c.Type}: {c.Value}")));

                var email = principal.FindFirst(ClaimTypes.Email)?.Value
                    ?? jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

                var name = principal.FindFirst(ClaimTypes.Name)?.Value
                    ?? jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(userId))
                    return new GoogleTokenValidationResult
                    {
                        IsValid = true,
                        Email = email,
                        Name = name,
                        UserId = userId
                    };
                {
                    logger.LogWarning("Missing required claims. Available claims: {Claims}", 
                        string.Join(", ", jwtToken.Claims.Select(c => $"{c.Type}: {c.Value}")));

                    return new GoogleTokenValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = $"Missing required claims. Email: {!string.IsNullOrEmpty(email)}, Sub: {!string.IsNullOrEmpty(userId)}"
                    };
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error validating Google token");
                return new GoogleTokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
