namespace LIN.Identity.Areas.Accounts;


[Route("account/logs")]
public class AccountLogsController : ControllerBase
{


    /// <summary>
    /// Obtienes la lista de accesos asociados a una cuenta
    /// </summary>
    /// <param name="token">Token de acceso</param>
    [HttpGet]
    public async Task<HttpReadAllResponse<LoginLogModel>> GetAll([FromHeader] string token)
    {

        // Token.
        var tokenInfo = Jwt.Validate(token);

        // Si el token no es valido.
        if (!tokenInfo.IsAuthenticated)
            return new()
            {
                Response = Responses.Unauthorized,
                Message = "Token invalido."
            };

        // Obtiene el usuario.
        var result = await Data.Logins.ReadAll(tokenInfo.AccountId);

        // Retorna el resultado.
        return result ?? new();

    }


}