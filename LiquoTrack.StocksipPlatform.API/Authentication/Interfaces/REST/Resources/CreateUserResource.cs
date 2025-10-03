using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources
{
    /// <summary>
    ///     Resource for the sign-up request
    /// </summary>
    public record CreateUserResource(Email Email, string Password, string Username, string UserRole);
}
