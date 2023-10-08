namespace LIN.Identity.Controllers;


[Route("Intents")]
public class IntentsController : ControllerBase
{

    /// <summary>
    /// Obtiene la lista de intentos Passkey activos
    /// </summary>
    /// <param name="token">Token de acceso</param>
    [HttpGet]
    public HttpReadAllResponse<PassKeyModel> GetAll([FromHeader] string token)
    {
        try
        {

            // Info del token
            var (isValid, user, _, _, _) = Jwt.Validate(token);

            // Si el token es invalido
            if (!isValid)
                return new ReadAllResponse<PassKeyModel>
                {
                    Message = "Invalid Token",
                    Response = Responses.Unauthorized
                };


            // Cuenta
            var account = (from a in PassKeyHub.Attempts
                           where a.Key == user.ToLower()
                           select a).FirstOrDefault().Value ?? new();

            // Hora actual
            var timeNow = DateTime.Now;

            // Intentos
            var intentos = (from I in account
                            where I.Status == PassKeyStatus.Undefined
                            where I.Expiración > timeNow
                            select I).ToList();

            // Retorna
            return new(Responses.Success, intentos);
        }
        catch
        {
            return new(Responses.Undefined);
        }
    }



}