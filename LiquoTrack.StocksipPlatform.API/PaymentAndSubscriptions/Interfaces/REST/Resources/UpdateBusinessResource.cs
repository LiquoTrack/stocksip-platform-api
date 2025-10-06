namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;

/// <summary>
///     The resource class for updating a business.
/// </summary>
public record UpdateBusinessResource(string AccountId, 
                                     string BusinessName,
                                     string BusinessEmail,
                                     string Ruc);