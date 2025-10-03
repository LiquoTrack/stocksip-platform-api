using System.Text.Json.Serialization;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources
{
    /// <summary>
    ///     The resource for the sign-in request
    /// </summary>
    public record SignInResource(string Email, string Password);
}
