namespace LiquoTrack.StocksipPlatform.API.ProfileManagement.Interfaces.REST.Resources;

/// <summary>
/// Resource for updating profile information.
/// </summary>
/// <param name="FirstName">The first name of the person.</param>
/// <param name="LastName">The last name of the person.</param>
/// <param name="PhoneNumber">The phone number.</param>
/// <param name="ProfilePictureUrl">The profile picture URL.</param>
public record UpdateProfileResource(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string ProfilePictureUrl
);