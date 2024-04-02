using System.Text.RegularExpressions;

namespace LIN.Cloud.Identity.Services.Formats;


public class Account
{



    /// <summary>
    /// Procesar el modelo.
    /// </summary>
    /// <param name="baseAccount">Modelo</param>
    public static (bool pass, string message) Validate(AccountModel baseAccount)
    {


        if (baseAccount.Name == null || baseAccount.Name.Trim().Length <= 0)
            return (false, "La cuenta debe de tener un nombre valido.");


        if (baseAccount.Identity == null || baseAccount.Identity.Unique == null|| baseAccount.Identity.Unique.Trim().Length <= 0)
            return (false, "La cuenta debe de tener una identidad unica valida.");



        if (!ValidarCadena(baseAccount.Identity.Unique))
            return (false, "La identidad de la cuenta no puede contener simbolos NO alfanumericos.");


        return (true, "");
    }



    static bool ValidarCadena(string cadena)
    {
        // Patrón de expresión regular para permitir solo letras o números
        string patron = "^[a-zA-Z0-9]*$";

        // Comprobar la coincidencia con el patrón
        return Regex.IsMatch(cadena, patron);
    }



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
            Creation = DateTime.Now,
            Name = baseAccount.Name.Trim(),
            Profile = baseAccount.Profile,
            Password = Global.Utilities.Cryptography.Encrypt(baseAccount.Password),
            Visibility = baseAccount.Visibility,
            IdentityId = 0,
            Identity = new()
            {
                Id = 0,
                Status = IdentityStatus.Enable,
                Type = IdentityType.Account,
                CreationTime = DateTime.Now,
                EffectiveTime = baseAccount.Identity.EffectiveTime == default ? DateTime.Now : baseAccount.Identity.EffectiveTime,
                ExpirationTime = baseAccount.Identity.ExpirationTime == default ? DateTime.Now.AddYears(5) : baseAccount.Identity.ExpirationTime,
                Roles = [],
                Unique = baseAccount.Identity.Unique.Trim()
            }
        };

    }


}