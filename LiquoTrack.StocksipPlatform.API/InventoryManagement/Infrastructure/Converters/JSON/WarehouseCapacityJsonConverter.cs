using System.Text.Json;
using System.Text.Json.Serialization;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Converters.JSON;

/// <summary>
///     Converter for the WarehouseCapacity value object.
/// </summary>
public class WarehouseCapacityJsonConverter : JsonConverter<WarehouseCapacity>
{
    public override WarehouseCapacity? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetDouble();
        return new WarehouseCapacity(value) ?? throw new InvalidOperationException();
    }

    public override void Write(Utf8JsonWriter writer, WarehouseCapacity value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.GetValue());
    }
}