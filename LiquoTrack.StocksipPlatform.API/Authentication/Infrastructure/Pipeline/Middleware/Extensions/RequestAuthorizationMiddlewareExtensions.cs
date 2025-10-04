﻿using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Pipeline.Middleware.Components;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Pipeline.Middleware.Extensions;

/// <summary>
/// RequestAuthorizationMiddlewareExtensions
/// This class includes a method extension to register RequestAuthorizationMiddleware in the ASP.NET Core pipeline.
/// </summary>
public static class RequestAuthorizationMiddlewareExtensions
{
    /// <summary>
    /// UseRequestAuthorization extension method is used to register RequestAuthorizationMiddleware in the ASP.NET Core pipeline.
    /// </summary>
    public static IApplicationBuilder UseRequestAuthorization(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestAuthorizationMiddleware>();
    }
}