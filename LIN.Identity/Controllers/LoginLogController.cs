namespace LIN.Identity.Controllers;


[Route("Account/logs")]
public class LoginLogController : ControllerBase
{


    /// <summary>
    /// Obtienes la lista de accesos asociados a una cuenta
    /// </summary>
    /// <param name="token">Token de acceso</param>
    [HttpGet("read/all")]
    public async Task<HttpReadAllResponse<LoginLogModel>> GetAll([FromHeader] string token)
    {

        // JWT
        var (isValid, _, userID, _, _) = Jwt.Validate(token);

        // Validación
        if (!isValid)
            return new(Responses.Unauthorized);

        // Obtiene el usuario
        var result = await Data.Logins.ReadAll(userID);

        // Retorna el resultado
        return result ?? new();

    }



}