﻿using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands
{
    /**
    * <summary>
    *     The sign in command
    * </summary>
    * <remarks>
    *     This command object includes the username and password to sign in
    * </remarks>
    */
    public record SignInCommand(Email Email, string Password);
}
