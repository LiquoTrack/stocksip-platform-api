using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Infrastructure.Persistence.MongoDB.Configuration.Serializers;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Infrastructure.Persistence.MongoDB.Configuration.ContextMapping;

/// <summary>
///     Static helper class for registering MongoDB mappings specific to the PaymentAndSubscriptions bounded context.
/// </summary>
public class PaymentAndSubscriptionsMappingHelper
{
    /// <summary>
    ///     Static method to register all PaymentAndSubscriptions related MongoDB mappings.
    /// </summary>
    public static void RegisterPaymentAndSubscriptionsMappings()
    {
        //
        // Use of Business-related Value Objects
        //
        SerializerRegistrationHelper.TryRegisterSerializer(new BusinessNameSerializer());
        SerializerRegistrationHelper.TryRegisterSerializer(new BusinessEmailSerializer());
        SerializerRegistrationHelper.TryRegisterSerializer(new RucSerializer());
        
        //
        // Use of the Account-related Value Objects
        //
        SerializerRegistrationHelper.TryRegisterSerializer(new EnumSerializer<EAccountRole>(BsonType.String));
        SerializerRegistrationHelper.TryRegisterSerializer(new EnumSerializer<EAccountStatuses>(BsonType.String));
        
        //
        // Use of the Plan-related Value Objects
        //
        SerializerRegistrationHelper.TryRegisterSerializer(new PlanLimitsSerializer());
        SerializerRegistrationHelper.TryRegisterSerializer(new EnumSerializer<EPlanType>(BsonType.String));
        
        //
        // Use of the Subscriptions-related Value Objects
        //        
        SerializerRegistrationHelper.TryRegisterSerializer(new EnumSerializer<ESubscriptionStatus>(BsonType.String));
        
    }
}