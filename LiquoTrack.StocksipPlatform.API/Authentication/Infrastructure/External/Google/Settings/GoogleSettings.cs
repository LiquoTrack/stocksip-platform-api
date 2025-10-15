namespace LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google.Settings
{
    /// <summary>
    /// This class is used to store the Google settings.
    /// It is used to configure the Google settings in the app settings .json file.
    /// </summary>
    public class GoogleSettings
    {
        /// <summary>
        /// The Google client ID.
        /// </summary>
        public string ClientId { get; set; } = default;
    }
}
