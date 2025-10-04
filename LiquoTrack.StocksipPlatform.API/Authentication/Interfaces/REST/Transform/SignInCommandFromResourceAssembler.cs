using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using System;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Transform
{
    /// <summary>
    ///     Static assembler class to convert SignInResource to SignInCommand. 
    /// </summary>
    public static class SignInCommandFromResourceAssembler
    {
        /// <summary>
        ///     Method to convert SignInResource to SignInCommand.
        /// </summary>
        /// <param name="resource">
        ///     The SignInResource to convert.
        /// </param>
        /// <returns>
        ///     The SignInCommand.
        /// </returns>
        public static SignInCommand ToCommandFromResource(SignInResource resource)
        {
            return new SignInCommand(resource.Email, resource.Password);
        }
    }
}
