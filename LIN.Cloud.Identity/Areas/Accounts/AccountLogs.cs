namespace LIN.Cloud.Identity.Areas.Accounts;

[Route("account/logs")]
public class AccountLogsController(Data.AccountLogs accountData) : AuthenticationController
{

    /// <summary>
    /// Obtener los logs asociados a una cuenta.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <returns>Lista de logs.</returns>
    [HttpGet]
    [IdentityToken]
    public async Task<HttpReadAllResponse<AccountLog>> ReadAll([FromHeader] string token)
    {

        // Obtiene el usuario
        var response = await accountData.ReadAll(AuthenticationInformation.AccountId);

        return response;

    }

}