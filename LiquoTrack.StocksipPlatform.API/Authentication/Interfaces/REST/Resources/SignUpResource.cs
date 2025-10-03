using System.Text.Json.Serialization;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources
{
    /// <summary>
    ///     Resource for the sign-up request
    /// </summary>
    public record SignUpResource(
        string Email,
        string Password,
        string Name,
        string BusinessName,
        string Role
    );
}
