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
        SharedMappingHelper.RegisterSharedMappings();
        
        // IAM Bounded Context
        
        // Inventory Bounded Context
        
        // Order Management Bounded Context
        
        // Procurement Ordering Bounded Context
        
        // Subscription Bounded Context
        
        // Alerts Bounded Context
        
        // Profiles Bounded Context
        
        _initialized = true;
    }
}