using System.Text.Json;
using System.Text.Json.Serialization;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Converters.JSON;

/// <summary>
///     This class is a custom JSON converter for the Email value object.
/// </summary>
public class EmailJsonConverter : JsonConverter<Email>
{
    public override Email? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return new Email(value ?? throw new InvalidOperationException());
    }

    public override void Write(Utf8JsonWriter writer, Email value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.GetValue);
    }
}