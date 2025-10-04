using System.Text.Json;
using System.Text.Json.Serialization;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Infrastructure.Converters.JSON;

/// <summary>
///     Converter for the EPlanType enum to JSON string representation.
/// </summary>
public class EPlanTypeJsonConverter : JsonConverter<EPlanType>
{
    public override EPlanType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return Enum.Parse<EPlanType>(value ?? throw new InvalidOperationException());
    }

    public override void Write(Utf8JsonWriter writer, EPlanType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}