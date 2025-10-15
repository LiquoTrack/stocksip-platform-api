namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Queries;

/// <summary>
///     Query to get a user by their ID.
/// </summary>
public record GetUserByIdQuery(string UserId);