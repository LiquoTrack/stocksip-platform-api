using System.Text.Json.Serialization;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources
{
    public record SignInResource(
        [property: JsonPropertyName("email")] string Email, 
        [property: JsonPropertyName("password")] string Password);
}
