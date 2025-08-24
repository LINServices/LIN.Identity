using LIN.Types.Models;
using IdentityService = LIN.Types.Cloud.Identity.Enumerations.IdentityService;

namespace LIN.Cloud.Identity.Services.Formats;

public class Account
{

    /// <summary>
    /// Procesar el modelo.
    /// </summary>
    /// <param name="baseAccount">Modelo</param>
    public static List<ErrorModel> Validate(AccountModel baseAccount)
    {

        List<ErrorModel> errors = [];

        if (string.IsNullOrWhiteSpace(baseAccount.Name))
            errors.Add(new ErrorModel()
            {
                Tittle = "Nombre invalido",
                Description = "El nombre del usuario no puede estar vacío.",
            });

        if (baseAccount.Identity == null || string.IsNullOrWhiteSpace(baseAccount.Identity.Unique))
            errors.Add(new ErrorModel()
            {
                Tittle = "Identidad no valida",
                Description = "La cuenta debe tener un identificador único valido.",
            });

        if (!ValidarCadena(baseAccount.Identity?.Unique))
            errors.Add(new ErrorModel()
            {
                Tittle = "Identidad no valida",
                Description = "La identidad de la cuenta no puede contener símbolos NO alfanuméricos."
            });

        return errors;
    }



    static bool ValidarCadena(string? cadena)
    {
        // Si la cadena es nula o vacía, no es válida
        if (string.IsNullOrWhiteSpace(cadena))
            return false;

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
                Provider = IdentityService.LIN,
                Unique = baseAccount.Identity.Unique.Trim()
            }
        };

    }


}