using LIN.Cloud.Identity.Services.Services;

namespace LIN.Cloud.Identity.Areas.Authentication;

[Route("V4/[controller]")]
public class AuthenticationV4Controller(IAuthenticationAccountService serviceAuth) : AuthenticationBaseController
{

    /// <summary>
    /// Iniciar sesión en una cuenta de usuario.
    /// </summary>
    /// <param name="user">Unique.</param>
    /// <param name="password">Contraseña.</param>
    /// <param name="token">token de la app..</param>
    /// <returns>Retorna el modelo de la cuenta y el token de acceso.</returns>
    [HttpGet("login")]
    public async Task<HttpReadOneResponse<AccountModel>> Login([FromQuery] string user, [FromQuery] string password, [FromHeader] string token)
    {

        // Validación de parámetros.
        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(token))
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son invalido."
            };

        // Validar el token
        int appId = JwtApplicationsService.Validate(token);

        if (appId <= 0)
            return new(Responses.Unauthorized)
            {
                Message = "El token no es valido."
            };

        // Establecer credenciales.
        var response = await serviceAuth.Authenticate(new()
        {
            User = user,
            Password = password,
            ApplicationId = appId
        });

        // Validación al obtener el usuario
        switch (response.Response)
        {
            // Correcto
            case Responses.Success:
                break;

            // No existe esta cuenta.
            case Responses.NotExistAccount:
                return new()
                {
                    Response = Responses.NotExistAccount,
                    Message = "No existe esta cuenta."
                };

            // Contraseña invalida.
            case Responses.InvalidPassword:
                return new()
                {
                    Response = Responses.InvalidPassword,
                    Message = "Contraseña incorrecta."
                };

            // Contraseña invalida.
            case Responses.UnauthorizedByApp:
                return new()
                {
                    Response = Responses.UnauthorizedByApp,
                    Message = "La aplicación no existe o no permite que inicies sesión en este momento.",
                    Errors = response.Errors
                };

            // Contraseña invalida.
            case Responses.UnauthorizedByOrg:
                return new()
                {
                    Response = Responses.UnauthorizedByOrg,
                    Message = "Tu organización no permite que inicies sesión en este momento.",
                    Errors = response.Errors
                };

            // Incorrecto
            default:
                return new()
                {
                    Response = Responses.Undefined,
                    Message = "Hubo un error grave."
                };
        }

        // Genera el token
        var tokenGen = serviceAuth.GenerateToken();

        // Respuesta.
        var http = new ReadOneResponse<AccountModel>
        {
            Model = serviceAuth.Account!,
            Response = Responses.Success,
            Token = tokenGen
        };

        return http;
    }

}