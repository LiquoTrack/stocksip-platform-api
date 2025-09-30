namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.ValueObjects;

/// <summary>
///     Record representing an account role.
/// </summary>
/// <param name="Role">
///     The role of the account.
/// </param>
/// <param name="DisplayName">
///     The display name of the role.
/// </param>
/// <param name="Description">
///     A description of the role.
/// </param>
public record AccountRole(EAccountRole Role, string DisplayName, string Description)
{
    /// <summary>
    ///     Default constructor for LiquorStoreOwner role.
    /// </summary>
    public static AccountRole LiquorStoreOwner => 
        new(EAccountRole.LiquorStoreOwner, "Liquor Store Owner", "Owner of a liquor store with access to manage inventory and sales.");
    
    /// <summary>
    ///     Default constructor for Supplier role.
    /// </summary>
    public static AccountRole Supplier => 
        new(EAccountRole.Supplier, "Supplier", "Supplier providing products to liquor stores.");
}