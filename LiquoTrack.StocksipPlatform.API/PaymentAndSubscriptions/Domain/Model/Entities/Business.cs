using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Entities;

/// <summary>
///     Entity that represents a Business.
/// </summary>
public class Business : Entity
{
    
    /// <summary>
    ///     The name of the business.
    /// </summary>
    public BusinessName BusinessName { get; private set; }
    
    /// <summary>
    ///     The main email of the business.
    /// </summary>
    public BusinessEmail? BusinessEmail { get; private set; }
    
    /// <summary>
    ///     The RUC of the business.
    /// </summary>
    public Ruc? Ruc { get; private set; }

    /// <summary>
    ///     Default constructor for Business.
    /// </summary>
    /// <param name="businessName">
    ///     The name of the business.
    /// </param>
    public Business(BusinessName businessName) => BusinessName = businessName;
    
    /// <summary>
    ///     Constructor for Business with all properties.
    /// </summary>
    /// <param name="businessName">
    ///     The name of the business.  
    /// </param>
    /// <param name="businessEmail">
    ///     The main email of the business. 
    /// </param>
    /// <param name="ruc">
    ///     The RUC of the business.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     The business name cannot be null.
    /// </exception>
    public Business(BusinessName businessName, BusinessEmail businessEmail, Ruc ruc)
    {
        BusinessName = businessName ?? throw new ArgumentNullException(nameof(businessName));
        BusinessEmail = businessEmail;
        Ruc = ruc;
    }
}