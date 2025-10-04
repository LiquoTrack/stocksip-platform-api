using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Transform
{
    public static class CreateUserCommandFromResourceAssembler
    {
        public static CreateUserCommand ToCommandFromResource(CreateUserResource resource)
        {
            return new CreateUserCommand(
                resource.Email, 
                resource.Password, 
                resource.Username, 
                resource.UserRole);
        }
    }
}