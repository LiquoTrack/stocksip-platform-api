using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using System;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Transform
{
    public static class SignInCommandFromResourceAssembler
    {
        public static SignInCommand ToCommandFromResource(SignInResource resource)
        {
            if (resource == null)
                throw new ArgumentNullException(nameof(resource));
                
            // Crear un nuevo objeto Email con el valor de cadena del correo
            var email = new Email(resource.Email);
            
            return new SignInCommand(email, resource.Password);
        }
    }
}
