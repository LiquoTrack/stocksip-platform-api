using System.Text.Json;
using System.Text.Json.Serialization;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Converters.JSON;

/// <summary>
///     This class is a custom JSON converter for the UserId value object.
/// </summary>
public class UserIdJsonConverter : JsonConverter<OwnerUserId>
{
    public override OwnerUserId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return new OwnerUserId(value ?? throw new InvalidOperationException());
    }

    public override void Write(Utf8JsonWriter writer, OwnerUserId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.GetId);
    }
}