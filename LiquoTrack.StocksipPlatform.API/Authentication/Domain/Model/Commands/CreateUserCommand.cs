using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands;

public record CreateUserCommand(Email Email, string Password, string Username,string UserRole);