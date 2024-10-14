namespace LIN.Cloud.Identity.Areas.Accounts;

[Route("account/logs")]
public class AccountLogsController(Data.AccountLogs accountData) : AuthenticationBaseController
{

    /// <summary>
    /// Obtener los logs asociados a una cuenta.
    /// </summary>
    /// <returns>Lista de logs.</returns>
    [HttpGet]
    [IdentityToken]
    public async Task<HttpReadAllResponse<AccountLog>> ReadAll()
    {

        // Obtiene el usuario
        var response = await accountData.ReadAll(AuthenticationInformation.AccountId);

        return response;

    }

}