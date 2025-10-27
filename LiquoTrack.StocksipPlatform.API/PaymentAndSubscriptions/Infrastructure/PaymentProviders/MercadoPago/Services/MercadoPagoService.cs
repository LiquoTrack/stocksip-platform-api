using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.OutBoundServices.PaymentProviders;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.OutBoundServices.PaymentProviders.models;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Infrastructure.PaymentProviders.MercadoPago.Configuration;
using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using Microsoft.Extensions.Options;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Infrastructure.PaymentProviders.MercadoPago.Services;

/// <summary>
///     MercadoPago service implementation.
/// </summary>
public class MercadoPagoService : IMercadoPagoService
{
    
    /// <summary>
    ///     MercadoPago settings.
    /// </summary>
    private readonly MercadoPagoSettings _settings;

    /// <summary>
    ///     Default constructor for MercadoPagoService.   
    /// </summary>
    /// <param name="settings">
    ///     The MercadoPago settings. 
    /// </param>
    public MercadoPagoService(IOptions<MercadoPagoSettings> settings)
    {
        _settings = settings.Value;
        MercadoPagoConfig.AccessToken = _settings.AccessToken;
    }
    
    /// <summary>
    ///     Method to create a payment preference.
    /// </summary>
    /// <param name="title">
    ///     The title of the payment preference.
    /// </param>
    /// <param name="price">
    ///     The price of the payment preference.
    /// </param>
    /// <param name="currency">
    ///     The currency of the payment preference.
    /// </param>
    /// <param name="quantity">
    ///     The quantity of the payment preference.
    /// </param>
    /// <returns>
    ///     A string representing the ID of the payment preference.
    /// </returns>
    public (string PreferenceId, string InitPoint) CreatePaymentPreference(string title, decimal price, string currency, int quantity)
    {
        var request = new PreferenceRequest
        {
            Items = new List<PreferenceItemRequest>
            {
                new PreferenceItemRequest
                {
                    Title = title,
                    Quantity = quantity,
                    CurrencyId = currency,
                    UnitPrice = price
                }
            },
            BackUrls = new PreferenceBackUrlsRequest
            {
                Success = "stocksip://payment/congrats",
                Failure = "stocksip://payment/failure",
                Pending = "stocksip://payment/pending"
            },
            AutoReturn = "approved"
        };
        
        var client = new PreferenceClient();
        var preference = client.CreateAsync(request).Result;
        return (preference.Id, preference.InitPoint);
    }
    
    public async Task<MercadoPagoPayment?> GetPaymentById(string paymentId)
    {
        if (!long.TryParse(paymentId, out long id))
            throw new ArgumentException("Invalid payment ID", nameof(paymentId));

        var client = new PaymentClient();
        Payment payment = await client.GetAsync(id);

        string preferenceId = payment.Order?.Id.ToString() 
                              ?? payment.AdditionalInfo?.Items?.FirstOrDefault()?.Id.ToString() 
                              ?? "";

        return new MercadoPagoPayment
        (
            payment.Id.ToString(),
            payment.Status,
            preferenceId
        );
    }
}