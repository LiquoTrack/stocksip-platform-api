using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration.Serializers;

namespace LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration.ContextMapping;

/// <summary>
///     This class provides helper methods for MongoDB mapping configurations.
///     It allows the mapping for value objects that are used inside a class.
/// </summary>
public static class SharedMappingHelper
{
    /// <summary>
    ///     This method registers all shared MongoDB mappings.
    ///     It should be called once during the application startup.
    /// </summary>
    public static void RegisterSharedMappings()
    {
        // Use of AccountId Value Object serializer
        SerializerRegistrationHelper.TryRegisterSerializer(new AccountIdSerializer());
    }
}