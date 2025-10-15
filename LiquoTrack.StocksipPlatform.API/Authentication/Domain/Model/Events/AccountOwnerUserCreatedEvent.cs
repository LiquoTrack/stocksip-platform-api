using CloudinaryDotNet.Actions;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Events;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Events;

public class AccountOwnerUserCreatedEvent: IDomainEvent
{
    public int Id { get; set; }
    public Email Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdateAt { get; set; }
    public required AccountId AccountId { get; set; }
    public Role UserRole { get; set; }
    public string UserRoleId { get; set; }
    public DateTime OccurredOn { get; }
}