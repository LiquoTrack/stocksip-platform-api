using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.OutBoundServices.PaymentProviders.models;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.OutBoundServices.PaymentProviders;

/// <summary>
///     Interface for MercadoPago service.
/// </summary>
public interface IMercadoPagoService
{
    /// <summary>
    ///     Method to create a payment preference in MercadoPago.
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
    (string PreferenceId, string InitPoint) CreatePaymentPreference(string title, decimal price, string currency, int quantity);
    
    Task<MercadoPagoPayment?> GetPaymentById(string paymentId);
}