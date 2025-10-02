using System.Text.Json;
using System.Text.Json.Serialization;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Infrastructure.Converters.JSON;

/// <summary>
///     Converter for the BusinessEmail value object to and from JSON.
/// </summary>
public class BusinessEmailJsonConverter : JsonConverter<BusinessEmail>
{
    public override BusinessEmail? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return new BusinessEmail(value!);
    }

    public override void Write(Utf8JsonWriter writer, BusinessEmail value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}