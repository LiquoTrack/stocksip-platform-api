using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Transform;

/// <summary>
///     Assembler class to convert UserWithProfile entity to UserWithProfileResource.
/// </summary>
public class UserWithProfileResourceFromEntityAssembler
{
    /// <summary>
    ///     Method to convert UserWithProfile entity to UserWithProfileResource.
    /// </summary>
    /// <param name="resource">
    ///     Resource to convert.
    /// </param>
    /// <returns>
    ///     A new instance of UserWithProfileResource.
    /// </returns>
    public static UserWithProfileResource ToResourceFromEntity(UserWithProfileResource resource)
    {
        return new UserWithProfileResource(
            resource.UserId,
            resource.Email,
            resource.Role,
            resource.ProfileId,
            resource.FullName,
            resource.PhoneNumber,
            resource.ProfilePictureUrl,
            resource.profileRole
        );
    }

    /// <summary>
    ///     Method to convert a list of UserWithProfile entities to a list of UserWithProfileResource.
    /// </summary>
    /// <param name="resources">
    ///     The list of UserWithProfile entities to convert.
    /// </param>
    /// <returns>
    ///     A list of UserWithProfileResource.
    /// </returns>
    public static IEnumerable<UserWithProfileResource> ToResourceListFromResourceList(IEnumerable<UserWithProfileResource> resources)
    {
        return resources.Select(ToResourceFromEntity);
    }
}