using System.Text.Json;
using System.Text.Json.Serialization;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Converters.JSON;

/// <summary>
///     Converter for the ProductContent value object to JSON string representation.
/// </summary>
public class ProductContentJsonConverter : JsonConverter<ProductContent>
{
    public override ProductContent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetDecimal();
        return new ProductContent(value);
    }

    public override void Write(Utf8JsonWriter writer, ProductContent value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.GetValue());
    }
}