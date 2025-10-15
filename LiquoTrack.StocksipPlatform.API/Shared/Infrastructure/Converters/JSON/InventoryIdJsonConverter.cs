using System.Text.Json;
using System.Text.Json.Serialization;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Converters.JSON;

/// <summary>
///     This class is a custom JSON converter for the InventoryId value object.
/// </summary>
public class InventoryIdJsonConverter : JsonConverter<InventoryId>
{
    public override InventoryId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return new InventoryId(value ?? throw new InvalidOperationException());
    }

    public override void Write(Utf8JsonWriter writer, InventoryId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.GetId);
    }
}