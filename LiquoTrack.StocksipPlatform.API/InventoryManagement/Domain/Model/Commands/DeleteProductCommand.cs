using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;

/// <summary>
///     Command to delete a product by its ID.
/// </summary>
public record DeleteProductCommand(ObjectId ProductId);