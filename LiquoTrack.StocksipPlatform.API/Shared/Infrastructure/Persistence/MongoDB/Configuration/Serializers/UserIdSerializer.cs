using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson.Serialization;

namespace LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration.Serializers;

/// <summary>
///     This class provides a custom serializer for the UserId value object to be used in MongoDB.
/// </summary>
public class UserIdSerializer : IBsonSerializer<UserId>
{
    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, UserId value)
    {
        context.Writer.WriteString(value.GetId);
    }

    public UserId Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var value = context.Reader.ReadString();
        return new UserId(value ?? throw new InvalidOperationException());
    }

    void IBsonSerializer.Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        => Serialize(context, args, (UserId)value);

    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        => Deserialize(context, args);

    public Type ValueType => typeof(UserId);
}