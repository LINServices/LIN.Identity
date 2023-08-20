using LIN.Auth.Hubs;

namespace LIN.Auth.Controllers;


[Route("Intents")]
public class IntentsController : ControllerBase
{

    /// <summary>
    /// Obtiene la lista de intentos Passkey activos
    /// </summary>
    /// <param name="contextDevice">Dispositivo de contexto</param>
    /// <param name="user">Usuario</param>
    [HttpGet]
    public HttpReadAllResponse<PassKeyModel> GetAll([FromHeader] string token)
    {
        try
        {


            var (isValid, _, userID) = Jwt.Validate(token);
           
            if (!isValid)
            {
                return new ReadAllResponse<PassKeyModel>
                {
                    Message = "Invalid Token",
                    Response = Responses.Unauthorized
                };
            }

            // Cuenta
            var account = (from A in PassKeyHub.Attempts
                           where A.Key == userID
                           select A).FirstOrDefault().Value ?? new();

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
        catch (Exception ex)
        {
            return new(Responses.Undefined);
        }
    }



}
