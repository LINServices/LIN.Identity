namespace LIN.Cloud.Identity.Services.Services.Authentication;

internal class SaveLogService(IAccountLogRepository accountLogRepository) : ISaveLogService
{
    public async Task<ResponseBase> Authenticate(AuthenticationRequest request)
    {
        if (request.Account == null)
            return new ResponseBase
            {
                Response = Responses.Undefined,
                Message = "La cuenta no ha sido definida."
            };

        if (request.Application == null)
            return new ResponseBase
            {
                Response = Responses.Undefined,
                Message = "La aplicación no ha sido definida."
            };

        if (request.Inpersonation)
        {
            return new ResponseBase(Responses.Success);
        }

        await accountLogRepository.Create(new()
        {
            AccountId = request.Account.Id,
            Application = request.ApplicationModel,
            AuthenticationMethod = request.AuthenticationMethod,
            Time = DateTime.UtcNow
        });

        return new ResponseBase(Responses.Success);
    }

}