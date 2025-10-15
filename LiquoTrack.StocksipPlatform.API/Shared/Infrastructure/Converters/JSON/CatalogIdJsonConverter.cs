using System.Text.Json;
using System.Text.Json.Serialization;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Converters.JSON;

/// <summary>
///     The JSON converter for the <see cref="CatalogId" /> value object.
/// </summary>
public class CatalogIdJsonConverter : JsonConverter<CatalogId>
{
    public override CatalogId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return new CatalogId(value ?? throw new InvalidOperationException());
    }

    public override void Write(Utf8JsonWriter writer, CatalogId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.GetId());
    }
}