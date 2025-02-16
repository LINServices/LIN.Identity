namespace LIN.Cloud.Identity.Areas.Accounts;

[IdentityToken]
[Route("account/logs")]
public class AccountLogsController(Data.AccountLogs accountData) : AuthenticationBaseController
{

    /// <summary>
    /// Obtener los logs asociados a una cuenta.
    /// </summary>
    /// <returns>Lista de logs.</returns>
    [HttpGet]
    public async Task<HttpReadAllResponse<AccountLog>> ReadAll(DateTime? start, DateTime? end)
    {

        // Fechas por defecto.
        start ??= DateTime.MinValue;
        end ??= DateTime.MaxValue;

        // Validar el rango de fecha.
        if (end < start)
            return new(Responses.InvalidParam)
            {
                Message = "La fecha de fin debe ser mayor a la fecha de inicio."
            };

        // Obtiene el usuario
        var response = await accountData.ReadAll(UserInformation.AccountId, start, end);
        return response;
    }

}