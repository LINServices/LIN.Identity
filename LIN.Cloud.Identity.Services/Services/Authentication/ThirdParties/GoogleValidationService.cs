using FirebaseAdmin.Auth;
using LIN.Types.Cloud.Identity.Models.Identities;

namespace LIN.Cloud.Identity.Services.Services.Authentication.ThirdParties;

public class GoogleValidationService(IAccountRepository accountRepository) : IGoogleValidationService
{

    /// <summary>
    /// Valida la cuenta de usuario y la contraseña.
    /// </summary>
    public async Task<ResponseBase> Authenticate(AuthenticationRequest request)
    {

        // Validar token de acceso.
        var information = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.ThirdPartyToken);

        // Validar que la identidad de usuario exista.
        string unique = information.Claims["email"].ToString() ?? string.Empty;
        string name = information.Claims["name"].ToString() ?? string.Empty;
        string? provider = information.Claims["firebase"] is Dictionary<string, object> firebaseClaims
                ? firebaseClaims["sign_in_provider"]?.ToString()
                : "unknown";

        if (string.IsNullOrEmpty(unique) || string.IsNullOrEmpty(name) || provider != "google.com")
            return new(Responses.Unauthorized)
            {
                Message = "El token es invalido o no esta firmado para Google."
            };
        
        var account = await accountRepository.Read(unique, new()
        {
            FindOn = Persistence.Models.FindOn.AllAccounts,
            IncludeIdentity = true
        });

        switch (account.Response)
        {
            case Responses.Success:
                break;
            case Responses.NotRows:
                var accountNew = new AccountModel()
                {
                    AccountType = AccountTypes.Personal,
                    Password = Global.Utilities.KeyGenerator.Generate(20, "pwd"),
                    Name = name,
                    Profile = "",
                    Visibility = Visibility.Visible,
                    Identity = new()
                    {
                        Unique = unique,
                        Type = IdentityType.Account
                    }
                };
                accountNew = Persistence.Formatters.Account.Process(accountNew);
                accountNew.IdentityService = Types.Cloud.Identity.Enumerations.IdentityService.Google;
                // Crear nueva identidad y cuenta.
                var create = await accountRepository.Create(accountNew, 0);
                account = create;
                break;
            default:
                return new()
                {
                    Response = Responses.InvalidUser,
                    Message = "Ocurrió un error al iniciar sesión con Google."
                };
        }

        if (account.Response != Responses.Success)
            return new(Responses.Unauthorized)
            {
                Message = "Ocurrió un error en LIN Platform & Google."
            };

        if (account.Model.IdentityService != Types.Cloud.Identity.Enumerations.IdentityService.Google)
            return new(Responses.Unauthorized)
            {
                Message = "La cuenta no esta vinculada con Google."
            };

        request.User = unique;
        return await Task.FromResult(new ResponseBase(Responses.Success));
    }

}