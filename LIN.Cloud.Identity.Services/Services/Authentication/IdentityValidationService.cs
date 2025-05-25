namespace LIN.Cloud.Identity.Services.Services.Authentication;

internal class IdentityValidationService(IAccountRepository accountRepository) : IIdentityValidationService
{

    /// <summary>
    /// Valida la cuenta de usuario y la contraseña.
    /// </summary>
    public async Task<ResponseBase> Authenticate(AuthenticationRequest request)
    {

        // Obtener la cuenta.
        var accountResponse = await accountRepository.Read(request.User, new()
        {
            IncludeIdentity = true,
            FindOn = Persistence.Models.FindOn.AllAccounts
        });

        // Validar respuesta.
        if (accountResponse.Response != Responses.Success)
            return new ResponseBase
            {
                Response = Responses.NotExistAccount,
                Message = "Account not found"
            };

        var account = accountResponse.Model;

        // Validar estado identidad.
        if (account.Identity.Status != IdentityStatus.Enable)
            return new ResponseBase
            {
                Response = Responses.NotExistAccount,
                Message = "La identidad de la cuenta de usuario no se encuentra activa."
            };

        if (request.StrictService && account.IdentityService != account.IdentityService)
            return new ResponseBase
            {
                Response = Responses.Unauthorized,
                Message = $"La cuenta no esta vinculada con el proveedor {request.Service}"
            };

        // Establecer datos en la solicitud.
        request.Account = account;

        return new(Responses.Success);
    }

}