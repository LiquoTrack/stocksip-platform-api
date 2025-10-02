using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Transform
{
    public static class SignUpCommandFromResourceAssembler
    {
        public static SignUpCommand ToCommandFromResource(SignUpResource resource)
        {
            if (resource == null) 
                throw new System.ArgumentNullException(nameof(resource));
                
            // Crear un nuevo objeto Email a partir del string
            var email = new Email(resource.Email);
            
            return new SignUpCommand(email, resource.Password, resource.Name);
        }
    }
}
