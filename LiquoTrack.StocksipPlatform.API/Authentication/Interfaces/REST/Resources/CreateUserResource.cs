using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources
{
    public record CreateUserResource(Email Email, string Password, string Username, Role UserRole);
}
