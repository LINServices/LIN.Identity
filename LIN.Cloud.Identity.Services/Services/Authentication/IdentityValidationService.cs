namespace LIN.Cloud.Identity.Services.Services.Authentication;

internal class IdentityValidationService(IAccountRepository accountRepository) : IIdentityValidationService
{

    /// <summary>
    /// Valida la existencia y el estado de la cuenta de usuario.
    /// </summary>
    public async Task<ResponseBase> Authenticate(AuthenticationRequest request)
    {

        var accountResponse = await accountRepository.Read(request.User, new()
        {
            IncludeIdentity = true,
            FindOn = Persistence.Models.FindOn.AllAccounts
        });

        if (accountResponse.Response != Responses.Success)
            return new ResponseBase
            {
                Response = Responses.NotExistAccount,
                Message = "Account not found"
            };

        var account = accountResponse.Model;

        if (account.Identity.Status != IdentityStatus.Enable)
            return new ResponseBase
            {
                Response = Responses.NotExistAccount,
                Message = "La identidad de la cuenta de usuario no se encuentra activa."
            };

        if (request.StrictService && account.Identity.Provider != request.Service)
            return new ResponseBase
            {
                Response = Responses.Unauthorized,
                Message = $"La cuenta no esta vinculada con el proveedor {request.Service}"
            };

        request.Account = account;

        return new(Responses.Success);
    }

}