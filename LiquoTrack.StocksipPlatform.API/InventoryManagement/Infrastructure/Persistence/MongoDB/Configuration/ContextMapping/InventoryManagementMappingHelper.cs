using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Persistence.MongoDB.Configuration.Serializers;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration.Serializers;
using MongoDB.Bson;
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
        //
        // Use of Product-related Value Objects
        //
        
        // Use of EBrandNames Value Object serializer
        SerializerRegistrationHelper.TryRegisterSerializer(new EnumSerializer<EBrandNames>(BsonType.String));
        
        // Use of EProductStates Value Object serializer
        SerializerRegistrationHelper.TryRegisterSerializer(new EnumSerializer<EProductTypes>(BsonType.String));
        
        // Use of EProductTypes Value Object serializer
        SerializerRegistrationHelper.TryRegisterSerializer(new EnumSerializer<EProductTypes>(BsonType.String));
        
        // Use of ProductContent Value Object serializer
        SerializerRegistrationHelper.TryRegisterSerializer(new ProductContentSerializer());
        
        // Use of ProductExpirationDate Value Object serializer
        SerializerRegistrationHelper.TryRegisterSerializer(new ProductExpirationDateSerializer());
        
        // Use of ProductMinimumStock Value Object serializer
        SerializerRegistrationHelper.TryRegisterSerializer(new ProductMinimumStockSerializer());
        
        // Use of ProductName Value Object serializer
        SerializerRegistrationHelper.TryRegisterSerializer(new ProductNameSerializer());
        
        // Use of ProductStock Value Object serializer
        SerializerRegistrationHelper.TryRegisterSerializer(new ProductStockSerializer());
        
        //
        // Use of Warehouse-related Value Object
        //
    }
}