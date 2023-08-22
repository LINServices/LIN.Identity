namespace LIN.Auth.Controllers;


[Route("mails")]
public class MailController : ControllerBase
{


    /// <summary>
    /// Obtiene los mails asociados a una cuenta
    /// </summary>
    /// <param name="token">Token de acceso</param>
    [HttpGet("all")]
    public async Task<HttpReadAllResponse<EmailModel>> GetMails([FromHeader] string token)
    {

        // Validación de JWT
        var (isValid, _, id, _) = Jwt.Validate(token);

        if (!isValid)
            return new(Responses.Unauthorized);
        
        return await Data.Mails.ReadAll(id);

    }



}