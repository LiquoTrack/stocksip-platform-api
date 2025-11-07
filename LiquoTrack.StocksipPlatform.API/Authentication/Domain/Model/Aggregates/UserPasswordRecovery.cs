namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;

public partial class User
{
    /// <summary>
    ///     The recovery password of the gmail service.
    /// </summary>
    public string? RecoveryPassword { get; set; }
    
    /// <summary>
    ///     Date when the recovery password expires.
    /// </summary>
    public DateTime? RecoveryPasswordExpiration { get; set; }

    /// <summary>
    ///     Method to set the recovery password.
    /// </summary>
    /// <param name="recoveryCode">
    ///     The 6-digit recovery code.
    /// </param>
    /// <param name="expiration">
    ///     The expiration time of the recovery code.
    /// </param>
    public void SetRecoveryCode(string recoveryCode, TimeSpan expiration)
    {
        RecoveryPassword = recoveryCode;
        RecoveryPasswordExpiration = DateTime.UtcNow.Add(expiration);
    }

    /// <summary>
    ///     Method to check if the recovery code is valid.
    /// </summary>
    /// <param name="code">
    ///     The recovery code to check.
    /// </param>
    /// <returns>
    ///     A boolean indicating whether the recovery code is valid.
    /// </returns>
    public bool IsRecoveryCodeValid(string code)
    {
        return RecoveryPassword == code && RecoveryPasswordExpiration > DateTime.UtcNow;
    }

    /// <summary>
    ///     Method to clear the recovery code.
    /// </summary>
    public void ClearRecoveryCode()
    {
        RecoveryPassword = null;
        RecoveryPasswordExpiration = null;   
    }

    /// <summary>
    ///     Method to update the password.
    /// </summary>
    /// <param name="newPassword">
    ///     The new password.
    /// </param>
    public void UpdatePassword(string newPassword) => Password = newPassword;

}