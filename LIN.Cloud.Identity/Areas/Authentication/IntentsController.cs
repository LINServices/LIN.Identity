using LIN.Cloud.Identity.Services.Realtime;

namespace LIN.Cloud.Identity.Areas.Authentication;

[IdentityToken]
[Route("[controller]")]
public class IntentsController(IAccountLogRepository passkeyData) : AuthenticationBaseController
{

    /// <summary>
    /// Obtiene la lista de intentos de llaves de paso están activos.
    /// </summary>
    [HttpGet]
    public HttpReadAllResponse<PassKeyModel> GetAll()
    {
        try
        {
            // Intentos del usuario actual.
            var account = (from a in PassKeyHub.Attempts
                           where a.Key.Equals(UserInformation.Unique, StringComparison.CurrentCultureIgnoreCase)
                           select a).FirstOrDefault().Value ?? [];

            var timeNow = DateTime.UtcNow;

            var intentos = (from I in account
                            where I.Status == PassKeyStatus.Undefined
                            where I.Expiration > timeNow
                            select I).ToList();

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
        var countResponse = await passkeyData.Count(UserInformation.AccountId);

        return countResponse;
    }

}