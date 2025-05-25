using IdentityService = LIN.Types.Cloud.Identity.Enumerations.IdentityService;

namespace LIN.Cloud.Identity.Persistence.Formatters;

public class Account
{

    /// <summary>
    /// Procesar el modelo.
    /// </summary>
    /// <param name="baseAccount">Modelo</param>
    public static AccountModel Process(AccountModel baseAccount)
    {
        return new AccountModel()
        {
            Id = 0,
            IdentityService = IdentityService.LIN,
            Name = baseAccount.Name.Trim(),
            Profile = baseAccount.Profile,
            Password = Global.Utilities.Cryptography.Encrypt(baseAccount.Password),
            Visibility = baseAccount.Visibility,
            IdentityId = 0,
            AccountType = baseAccount.AccountType,
            Identity = new()
            {
                Id = 0,
                Status = IdentityStatus.Enable,
                Type = IdentityType.Account,
                CreationTime = DateTime.UtcNow,
                EffectiveTime = DateTime.UtcNow,
                ExpirationTime = DateTime.UtcNow.AddYears(5),
                Roles = [],
                Unique = baseAccount.Identity.Unique.Trim()
            }
        };
    }

}