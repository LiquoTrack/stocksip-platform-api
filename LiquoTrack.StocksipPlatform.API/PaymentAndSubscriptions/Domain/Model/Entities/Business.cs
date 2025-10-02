using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Entities;

/// <summary>
///     Entity that represents a Business.
/// </summary>
public class Business(
    BusinessName businessName,
    BusinessEmail businessEmail,
    Ruc ruc
    ) : Entity
{
    /// <summary>
    ///     The name of the business.
    /// </summary>
    public BusinessName BusinessName { get; set; } = businessName;
    
    /// <summary>
    ///     The main email of the business.
    /// </summary>
    public BusinessEmail BusinessEmail { get; set; } = businessEmail;
    
    /// <summary>
    ///     The RUC of the business.
    /// </summary>
    public Ruc Ruc { get; private set; } = ruc;
}