using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Infrastructure.Persistence.MongoDB.Configuration.Serializers;

public class PurchaseOrderItemSerializer : IBsonSerializer<PurchaseOrderItem>
{
    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, PurchaseOrderItem value)
    {
        context.Writer.WriteStartDocument();
        context.Writer.WriteName("productId");
        context.Writer.WriteString(value.ProductId.GetId);
        context.Writer.WriteName("unitPrice");
        context.Writer.WriteDecimal128((decimal)value.UnitPrice);
        context.Writer.WriteName("quantity");
        context.Writer.WriteInt32(value.Quantity);
        context.Writer.WriteEndDocument();
    }

    public PurchaseOrderItem Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        context.Reader.ReadStartDocument();

        var productId = context.Reader.ReadString("productId");
        var unitPrice = (decimal)context.Reader.ReadDecimal128("unitPrice");
        var quantity = context.Reader.ReadInt32("quantity");

        context.Reader.ReadEndDocument();

        return new PurchaseOrderItem(new ProductId(productId), unitPrice, quantity);
    }

    void IBsonSerializer.Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        => Serialize(context, args, (PurchaseOrderItem)value);

    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        => Deserialize(context, args);

    public Type ValueType => typeof(PurchaseOrderItem);
}