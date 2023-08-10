namespace LIN.Auth.Controllers;


[Route("Account/logs")]
public class LoginLogController : ControllerBase
{


    /// <summary>
    /// Obtienes toda la lista de accesos asociados a una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    [HttpGet("read/all")]
    public async Task<HttpReadAllResponse<LoginLogModel>> GetAll([FromHeader] int id)
    {

        // Comprobaciones
        if (id <= 0)
            return new(Responses.InvalidParam);

        // Obtiene el usuario
        var result = await Data.Logins.ReadAll(id);

        // Retorna el resultado
        return result ?? new();

    }



}