using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google.Responses;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Authentication
{
    /// <summary>
    /// This interface is used to validate the Google authentication.
    /// It is used to validate the Google authentication in the app settings .json file.
    /// </summary>
    public interface IExternalAuthService
    {
        Task<ExternalAuthResult> ValidateIdTokenAsync(string idToken);
    }
}
