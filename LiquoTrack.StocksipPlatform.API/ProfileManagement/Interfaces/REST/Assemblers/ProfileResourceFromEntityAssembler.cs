using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.ProfileManagement.Interfaces.REST.Assemblers;

public static class ProfileResourceFromEntityAssembler
{
    /// <summary>
    /// Converts a Profile entity to a ProfileResource.
    /// </summary>
    /// <param name="profile">The profile entity.</param>
    /// <returns>The profile resource.</returns>
    public static ProfileResource ToResourceFromEntity(Profile profile)
    {
        return new ProfileResource(
            profile.Id.ToString(),
            profile.Name.FirstName,
            profile.Name.LastName,
            profile.FullName,
            profile.PersonContactNumber.PhoneNumber,
            profile.ContactNumber,
            profile.ProfilePictureUrl.ToString(),
            profile.UserId
        );
    }
}