using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;

/// <summary>
///     Query to get all products for a given supplier.
/// </summary>
public record GetAllProductsBySupplierIdQuery(AccountId SupplierId);