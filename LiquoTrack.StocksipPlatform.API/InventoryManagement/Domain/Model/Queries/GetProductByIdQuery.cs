using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;

/// <summary>
///     Query to get a product by its ID
/// </summary>
public record GetProductByIdQuery(ObjectId ProductId);