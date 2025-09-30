using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands
{
    /**
    * <summary>
    *     The sign up command
    * </summary>
    * <remarks>
    *     This command object includes the username and password to sign up
    * </remarks>
    */
    public record SignUpCommand(Email Email, string Password, string Name);
}
