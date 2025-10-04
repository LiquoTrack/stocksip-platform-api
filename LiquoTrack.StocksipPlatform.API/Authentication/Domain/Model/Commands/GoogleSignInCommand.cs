using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands
{
    public record GoogleSignInCommand(
        string IdToken,
        string? ClientId = null
    );
}
