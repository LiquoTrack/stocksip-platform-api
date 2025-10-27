using System.Text.Json;
using System.Text.Json.Serialization;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Infrastructure.Converters.JSON;

public class PurchaseOrderItemJsonConverter : JsonConverter<PurchaseOrderItem>
{
    public override PurchaseOrderItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (var jsonDoc = JsonDocument.ParseValue(ref reader))
        {
            var root = jsonDoc.RootElement;

            var productId = new ProductId(root.GetProperty("productId").GetString()!);
            var unitPrice = root.GetProperty("unitPrice").GetDecimal();
            var quantity = root.GetProperty("quantity").GetInt32();

            return new PurchaseOrderItem(productId, unitPrice, quantity);
        }
    }

    public override void Write(Utf8JsonWriter writer, PurchaseOrderItem value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("productId", value.ProductId.GetId);
        writer.WriteNumber("unitPrice", value.UnitPrice);
        writer.WriteNumber("quantity", value.Quantity);
        writer.WriteEndObject();
    }
}