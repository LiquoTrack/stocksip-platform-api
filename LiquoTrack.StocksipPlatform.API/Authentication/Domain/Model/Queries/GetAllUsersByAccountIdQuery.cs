using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Queries;

public record GetAllUsersByAccountIdQuery(AccountId AccountId);