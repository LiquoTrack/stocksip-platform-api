using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;

/// <summary>
///     Command to update product information.
/// </summary>
public record UpdateProductInformationCommand(
        string Name,
        Money UnitPrice,
        ProductMinimumStock MinimumStock,
        ImageUrl ImageUrl
    );