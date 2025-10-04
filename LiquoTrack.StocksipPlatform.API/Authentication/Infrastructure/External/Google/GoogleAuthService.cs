using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Authentication;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google.Responses;
using Microsoft.Extensions.Options;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google
{       
    /// <summary>
    /// This class is used to validate the Google authentication.
    /// It is used to validate the Google authentication in the app settings .json file.
    /// </summary>
    public class GoogleAuthService : IExternalAuthService
    {
        private readonly IGoogleTokenValidator _tokenValidator;
        private readonly ILogger<GoogleAuthService> _logger;

        /// <summary>
        /// Constructor of the GoogleAuthService class.
        /// </summary>
        /// <param name="tokenValidator">The Google token validator.</param>
        /// <param name="logger">The logger.</param>
        public GoogleAuthService(IGoogleTokenValidator tokenValidator, ILogger<GoogleAuthService> logger)
        {
            _tokenValidator = tokenValidator;
            _logger = logger;
        }

        /// <summary>
        /// Validates the Google ID token.
        /// </summary>
        /// <param name="idToken">The Google ID token.</param>
        /// <returns>The external authentication result.</returns>
        public async Task<ExternalAuthResult> ValidateIdTokenAsync(string idToken)
        {
            try
            {
                var validationResult = await _tokenValidator.ValidateIdTokenAsync(idToken);
                
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Invalid Google ID token: {Error}", validationResult.ErrorMessage);
                    return new ExternalAuthResult { Success = false, Error = validationResult.ErrorMessage };
                }

                return new ExternalAuthResult
                {
                    Success = true,
                    Email = validationResult.Email,
                    ProviderUserId = validationResult.UserId,
                    Name = validationResult.Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Google ID token");
                return new ExternalAuthResult 
                { 
                    Success = false, 
                    Error = $"Error validating Google ID token: {ex.Message}" 
                };
            }
        }
    }
}
