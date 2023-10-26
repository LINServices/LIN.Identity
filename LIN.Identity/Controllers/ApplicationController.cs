namespace LIN.Identity.Controllers;


[Route("application")]
public class ApplicationController : ControllerBase
{


    /// <summary>
    /// Obtiene los mails asociados a una cuenta
    /// </summary>
    /// <param name="token">Token de acceso</param>
    [HttpPost("create")]
    public async Task<HttpCreateResponse> GetMails([FromBody] ApplicationModel applicationModel)
    {

        return await Data.Applications.Create(applicationModel);

    }



}