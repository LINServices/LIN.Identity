namespace LIN.Cloud.Identity.Areas.Accounts;

[Route("account/logs")]
public class AccountLogsController(Data.AccountLogs accountData) : ControllerBase
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

        // Token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

        // Obtiene el usuario
        var response = await accountData.ReadAll(tokenInfo.AccountId);

        return response;

    }

}