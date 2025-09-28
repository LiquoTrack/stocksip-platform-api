using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services;

public interface IUserCommandService
{
    Task<User?> Handle(CreateUserCommand command);
    Task<User> CreateOrUpdateFromExternalAsync(string providerUserId, string email, string? name, string? accountId = null);
}