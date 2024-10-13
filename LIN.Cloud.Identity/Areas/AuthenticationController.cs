namespace LIN.Cloud.Identity.Areas;

public class AuthenticationController : ControllerBase
{

    /// <summary>
    /// Información de autenticación.
    /// </summary>
    public JwtModel AuthenticationInformation => base.HttpContext.Items["authentication"] as JwtModel ?? new();

}