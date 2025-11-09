namespace LIN.Cloud.Identity.Services.Services.Authentication;

internal class SaveLogService(IAccountLogRepository accountLogRepository) : ISaveLogService
{
    public async Task<ResponseBase> Authenticate(AuthenticationRequest request)
    {

        // Si no hay cuenta o aplicación, no se puede guardar el log.
        if (request.Account == null)
            return new ResponseBase
            {
                Response = Responses.Undefined,
                Message = "La cuenta no ha sido definida."
            };

        // Si no hay aplicación, no se puede guardar el log.
        if (request.Application == null)
            return new ResponseBase
            {
                Response = Responses.Undefined,
                Message = "La aplicación no ha sido definida."
            };

        // Guardar el log de autenticación.
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