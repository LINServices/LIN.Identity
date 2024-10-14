namespace LIN.Cloud.Identity.Areas;

public class AuthenticationBaseController : ControllerBase
{

    /// <summary>
    /// Información de autenticación.
    /// </summary>
    public JwtModel AuthenticationInformation => base.HttpContext.Items["authentication"] as JwtModel ?? new();


    /// <summary>
    /// Obtener el token desde el header.
    /// </summary>
    public string Token => base.HttpContext.Request.Headers["token"].ToString() ?? string.Empty;

}