using System.Threading.Tasks;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google;

public interface IGoogleTokenValidator
{
    /// <summary>
    /// Validates the Google token.
    /// </summary>
    /// <param name="idToken">The Google token.</param>
    /// <returns>The Google token validation result.</returns>
    Task<GoogleTokenValidationResult> ValidateIdTokenAsync(string idToken);
}

public class GoogleTokenValidationResult
{
    public bool IsValid { get; set; }
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? ErrorMessage { get; set; }
}
