namespace LIN.Cloud.Identity.Services.Services.Authentication;

internal class AccountValidationService : IAccountValidationService
{

    /// <summary>
    /// Valida la cuenta de usuario y la contraseña.
    /// </summary>
    public async Task<ResponseBase> Authenticate(AuthenticationRequest request)
    {

        // Validar contraseña.
        if (Global.Utilities.Cryptography.Encrypt(request.Password) != request.Account!.Password)
            return new ResponseBase
            {
                Response = Responses.InvalidPassword,
                Message = "La contraseña es incorrecta."
            };

        return await Task.FromResult(new ResponseBase(Responses.Success));
    }

}