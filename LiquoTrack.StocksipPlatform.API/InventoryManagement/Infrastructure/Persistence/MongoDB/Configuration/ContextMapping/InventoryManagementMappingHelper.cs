using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Persistence.MongoDB.Configuration.ContextMapping;

/// <summary>
///     Static helper class for registering MongoDB mappings specific to Inventory Management.
/// </summary>
public static class InventoryManagementMappingHelper
{
    /// <summary>
    ///     Static method to register all Inventory Management related MongoDB mappings.
    /// </summary>
    public static void RegisterInventoryManagementMappings()
    {
        // Use of EBrandNames Value Object serializer
        SerializerRegistrationHelper.TryRegisterSerializer(new EnumSerializer<EBrandNames>(BsonType.String));
    }
}