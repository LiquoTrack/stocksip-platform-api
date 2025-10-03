namespace LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google.Settings;

/// <summary>
/// This class is used to store the Google authentication settings.
/// It is used to configure the Google authentication settings in the app settings .json file.
/// </summary>
public class GoogleAuthSettings
{
    /// <summary>
    /// The Google client ID.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// The Google client secret.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;
}
