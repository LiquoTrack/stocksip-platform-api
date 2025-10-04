using System;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Persistence.MongoDB.Configuration.Serializers;

public class AccountIdSerializer : SerializerBase<AccountId>
{
    public override AccountId Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var type = context.Reader.GetCurrentBsonType();
        if (type == BsonType.String)
        {
            var value = context.Reader.ReadString();
            return new AccountId(value);
        }

        throw new InvalidOperationException($"Cannot deserialize AccountId from BsonType {type}");
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, AccountId value)
    {
        if (value != null)
        {
            var idProperty = typeof(AccountId).GetProperty("Id", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            var idValue = (string)idProperty.GetValue(value);
            context.Writer.WriteString(idValue);
        }
        else
        {
            context.Writer.WriteNull();
        }
    }
}
