namespace LIN.Identity.Areas.V3;


[Route("v3/Account/logs")]
public class LoginLogController : ControllerBase
{


    /// <summary>
    /// Obtienes la lista de accesos asociados a una cuenta
    /// </summary>
    /// <param name="token">Token de acceso</param>
    [HttpGet]
    public async Task<HttpReadAllResponse<LoginLogModel>> GetAll([FromHeader] string token)
    {

        // JWT.
        var (isValid, _, userId, _, _) = Jwt.Validate(token);

        // Validación.
        if (!isValid)
            return new(Responses.Unauthorized)
            {
                Message = "Token invalido."
            };

        // Obtiene el usuario.
        var result = await Data.Logins.ReadAll(userId);

        // Retorna el resultado.
        return result ?? new();

    }



}