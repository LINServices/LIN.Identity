using LIN.Cloud.Identity.Services.Realtime;

namespace LIN.Cloud.Identity.Areas.Authentication;

[IdentityToken]
[Route("[controller]")]
public class IntentsController(Data.PassKeys passkeyData) : AuthenticationBaseController
{

    /// <summary>
    /// Obtiene la lista de intentos de llaves de paso están activos.
    /// </summary>
    [HttpGet]
    public HttpReadAllResponse<PassKeyModel> GetAll()
    {
        try
        {
            // Cuenta
            var account = (from a in PassKeyHub.Attempts
                           where a.Key.Equals(UserInformation.Unique, StringComparison.CurrentCultureIgnoreCase)
                           select a).FirstOrDefault().Value ?? [];

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
        catch (Exception)
        {
            return new(Responses.Undefined)
            {
                Message = "Hubo un error al obtener los intentos de passkey"
            };
        }
    }


    /// <summary>
    /// Obtiene la lista de intentos de llaves de paso están activos.
    /// </summary>
    [HttpGet("count")]
    public async Task<HttpReadOneResponse<int>> Count()
    {
        // Contar.
        var countResponse = await passkeyData.Count(UserInformation.AccountId);

        // Retorna
        return countResponse;
    }

}