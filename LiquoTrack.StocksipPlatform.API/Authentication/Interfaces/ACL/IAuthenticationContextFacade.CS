namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.ACL
{
    public interface IAuthenticationContextFacade
    {
        Task<string> CreateUserAsync(string username, string password);
    }
}
