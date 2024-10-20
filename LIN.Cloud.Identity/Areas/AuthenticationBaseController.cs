namespace LIN.Cloud.Identity.Areas;

public class AuthenticationBaseController : ControllerBase
{

    /// <summary>
    /// Información de autenticación.
    /// </summary>
    public JwtModel AuthenticationInformation => HttpContext.Items["authentication"] as JwtModel ?? new();


    /// <summary>
    /// Obtener el token desde el header.
    /// </summary>
    public string Token => HttpContext.Request.Headers["token"].ToString() ?? string.Empty;

}