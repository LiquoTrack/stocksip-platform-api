using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services
{
    public interface IGoogleAuthService
    {
        Task<(User? user, string? token, string? error)> AuthenticateWithGoogleAsync(string idToken, string? clientId = null);
    }
}
