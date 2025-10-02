namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Commands;


/// <summary>
///     Command to create a new account.
/// </summary>
/// <param name="BusinessName">
///     The name of the business.
/// </param>
/// <param name="BusinessEmail">
///     The main email of the business.
/// </param>
/// <param name="Ruc">
///     The RUC of the business.
/// </param>
/// <param name="AccountRole">
///     The role of the account.
/// </param>
/// <param name="OwnerUserId">
///     The User Id of the owner of the account.
/// </param>
public record CreateAccountCommand(
        string BusinessName,
        string BusinessEmail,
        string Ruc,
        string AccountRole,
        string OwnerUserId
    );