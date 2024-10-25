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
    public async Task<HttpReadAllResponse<AccountLog>> ReadAll()
    {
        // Obtiene el usuario
        var response = await accountData.ReadAll(UserInformation.AccountId);
        return response;
    }

}