using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Entities;


/// <summary>
/// Represents a role in the publishing system.
/// </summary>
/// <param name="">
/// The name of the role.
/// </param>
public class Role
{
    public int Id { get; set; }
    public EUserRoles Name { get; set; } = EUserRoles.Normal;

}