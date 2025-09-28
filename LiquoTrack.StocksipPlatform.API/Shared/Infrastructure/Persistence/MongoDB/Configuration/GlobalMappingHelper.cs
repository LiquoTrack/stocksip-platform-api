using LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Persistence.MongoDB.Configuration.ContextMapping;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration.ContextMapping;

namespace LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;

/// <summary>
///     This class groups all MongoDB mapping helpers for different bounded contexts.
/// </summary>
public static class GlobalMongoMappingHelper
{
    private static bool _initialized = false;

    /// <summary>
    ///     Registers all MongoDB mappings for all bounded contexts.
    /// </summary>
    public static void RegisterAllBoundedContextMappings()
    {
        if (_initialized) return;
        
        // Shared Bounded Context
        Console.WriteLine("Registering Shared Mappings...");
        SharedMappingHelper.RegisterSharedMappings();
        Console.WriteLine("Shared Mappings Registered!");
        
        // IAM Bounded Context
        
        // Inventory Bounded Context
        Console.WriteLine("Registering Inventory Management Mappings...");
        InventoryManagementMappingHelper.RegisterInventoryManagementMappings();
        Console.WriteLine("Inventory Management Mappings Registered!");
        
        // Order Management Bounded Context
        
        // Procurement Ordering Bounded Context
        
        // Subscription Bounded Context
        
        // Alerts Bounded Context
        
        // Profiles Bounded Context
        
        _initialized = true;
    }
}