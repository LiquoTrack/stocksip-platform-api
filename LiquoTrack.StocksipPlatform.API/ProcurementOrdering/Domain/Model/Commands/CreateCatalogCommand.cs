namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;

/// <summary>
/// Command to create a new catalog.
/// </summary>
/// <param name="name">The name of the catalog.</param>
/// <param name="description">The description of the catalog.</param>
/// <param name="ownerAccount">The account identifier of the catalog owner.</param>
/// <param name="contactEmail">The contact email for the catalog.</param>
public record CreateCatalogCommand(string name, string description, string ownerAccount, string contactEmail);
