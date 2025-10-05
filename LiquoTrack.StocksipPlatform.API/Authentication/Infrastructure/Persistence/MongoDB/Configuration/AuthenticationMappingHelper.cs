using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Persistence.MongoDB.Configuration;

public static class AuthenticationMappingHelper
{
    public static void RegisterAuthenticationMappings()
    {
        //
        //  Use of User-related ValueObject
        //
        
        SerializerRegistrationHelper.TryRegisterSerializer(new EnumSerializer<EUserRoles>(BsonType.String));
    }
}