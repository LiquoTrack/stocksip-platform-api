namespace LiquoTrack.StocksipPlatform.API.ProfileManagement.Interfaces.REST.Resources;

/// <summary>
/// Resource for creating a new profile.
/// </summary>
/// <param name="FirstName">The first name of the person.</param>
/// <param name="LastName">The last name of the person.</param>
/// <param name="PhoneNumber">The phone number.</param>
/// <param name="ProfilePictureUrl">The profile picture URL.</param>
/// <param name="UserId">The user ID associated with the profile.</param>
/// <param name="AssignedRole">The assigned role in the profile.</param>
public record CreateProfileResource(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string ProfilePictureUrl,
    string UserId,
    string AssignedRole
);