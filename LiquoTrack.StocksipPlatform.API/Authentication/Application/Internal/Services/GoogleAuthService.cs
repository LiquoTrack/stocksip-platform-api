using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.CommandHandlers;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly GoogleSignInCommandHandler _googleSignInHandler;

        public GoogleAuthService(GoogleSignInCommandHandler googleSignInHandler)
        {
            _googleSignInHandler = googleSignInHandler;
        }

        public async Task<(User? user, string? token, string? error)> AuthenticateWithGoogleAsync(string idToken, string? clientId = null)
        {
            var command = new GoogleSignInCommand(idToken, clientId);
            return await _googleSignInHandler.Handle(command);
        }
    }
}
