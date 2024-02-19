using LIN.Cloud.Identity.Services.Realtime;

namespace LIN.Cloud.Identity.Areas.Authentication;


[Route("Intents")]
public class IntentsController : ControllerBase
{

    /// <summary>
    /// Obtiene la lista de intentos de llaves de paso están activos.
    /// </summary>
    /// <param name="token">Token de acceso</param>
    [HttpGet]
    [IdentityToken]
    public HttpReadAllResponse<PassKeyModel> GetAll([FromHeader] string token)
    {
        try
        {

            // Token.
            JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();
           
            // Cuenta
            var account = (from a in PassKeyHub.Attempts
                           where a.Key == tokenInfo.Unique.ToLower()
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
            return new(Responses.Undefined)
            {
                Message = "Hubo un error al obtener los intentos de passkey"
            };
        }
    }



}