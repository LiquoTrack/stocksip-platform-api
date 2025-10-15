namespace LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google.Requests
{
    /// <summary>
    /// This class is used to store the Google authentication request.
    /// It is used to configure the Google authentication request in the app settings .json file.
    /// </summary>
    public class GoogleAuthRequest
    {
        /// <summary>
        /// The ID Token that Google returns after a successful login.
        /// </summary>
        public string IdToken { get; set; }

        /// <summary>
        /// Access token if you also return it from the frontend.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Information of the client/user that you send along with the token.
        /// </summary>
        public string ClientId { get; set; }
    }
}
