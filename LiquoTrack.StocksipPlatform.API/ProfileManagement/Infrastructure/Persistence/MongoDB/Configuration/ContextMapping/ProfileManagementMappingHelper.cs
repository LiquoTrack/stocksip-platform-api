using LiquoTrack.StocksipPlatform.API.ProfileManagement.Infrastructure.Persistence.MongoDB.Configuration.Serializers;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration.Serializers;

namespace LiquoTrack.StocksipPlatform.API.ProfileManagement.Infrastructure.Persistence.MongoDB.Configuration.ContextMapping;

/// <summary>
///     Static helper class for registering MongoDB mappings specific to Profile Management.
/// </summary>
public static class ProfileManagementMappingHelper
{
    /// <summary>
    ///     Static method to register all Profile Management related MongoDB mappings.
    /// </summary>
    public static void RegisterProfileManagementMappings()
    {
        //
        // Use of Profile-related Value Objects
        //

        // PersonName Value Object serializer
        SerializerRegistrationHelper.TryRegisterSerializer(new PersonNameSerializer());

        // PersonContactNumber Value Object serializer
        SerializerRegistrationHelper.TryRegisterSerializer(new PersonContactNumberSerializer());
        
    }
}