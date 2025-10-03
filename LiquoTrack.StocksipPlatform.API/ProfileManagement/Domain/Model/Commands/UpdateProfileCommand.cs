using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.Commands;

/// <summary>
/// Command to update profile information.
/// </summary>
/// <param name="ProfileId">The ID of the profile to update.</param>
/// <param name="Name">The updated name of the person.</param>
/// <param name="PersonContactNumber">The updated contact number.</param>
/// <param name="ContactNumber">The contact number as string.</param>
/// <param name="ProfilePictureUrl">The updated profile picture URL.</param>
public record UpdateProfileCommand(
    string ProfileId,
    PersonName Name,
    PersonContactNumber PersonContactNumber,
    string ContactNumber,
    ImageUrl ProfilePictureUrl
);