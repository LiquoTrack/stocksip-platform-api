using System.Text.Json;
using System.Text.Json.Serialization;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Converters.JSON;

/// <summary>
///     The JSON converter for the <see cref="PurchaseOrderId" /> value object.
/// </summary>
public class PurchaseOrderIdJsonConverter : JsonConverter<PurchaseOrderId>
{
    public override PurchaseOrderId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return new PurchaseOrderId(value ?? throw new InvalidOperationException());
    }

    public override void Write(Utf8JsonWriter writer, PurchaseOrderId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.GetId);
    }
}