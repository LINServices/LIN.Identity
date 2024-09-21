using LIN.Cloud.Identity.Services.Auth.Interfaces;

namespace LIN.Cloud.Identity.Areas.Authentication;


[Route("[controller]")]
public class AuthenticationController(IAuthentication authentication, Data.Accounts accountData) : ControllerBase
{

    /// <summary>
    /// Inicia una sesión de usuario.
    /// </summary>
    /// <param name="user">Usuario único.</param>
    /// <param name="password">Contraseña del usuario.</param>
    /// <param name="application">Key de aplicación.</param>
    [HttpGet("login")]
    public async Task<HttpReadOneResponse<AccountModel>> Login([FromQuery] string user, [FromQuery] string password, [FromHeader] string application)
    {

        // Validación de parámetros.
        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(application))
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son invalido."
            };

        // Establecer credenciales.
        authentication.SetCredentials(user, password, application);

        // Respuesta.
        var response = await authentication.Start();

        // Validación al obtener el usuario
        switch (response)
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

            // Incorrecto
            default:
                return new()
                {
                    Response = Responses.Undefined,
                    Message = "Hubo un error grave."
                };
        }

        // Genera el token
        var token = authentication.GenerateToken();

        // Respuesta.
        var http = new ReadOneResponse<AccountModel>
        {
            Model = authentication.GetData(),
            Response = Responses.Success,
            Token = token
        };

        return http;
    }


    /// <summary>
    /// Inicia una sesión de usuario por medio del token.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    [HttpGet("LoginWithToken")]
    [IdentityToken]
    public async Task<HttpReadOneResponse<AccountModel>> LoginWithToken([FromHeader] string token)
    {

        // Token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

        // Obtiene el usuario.
        var response = await accountData.Read(tokenInfo.AccountId, new QueryAccountFilter()
        {
            IsAdmin = true,
            FindOn = FindOn.StableAccounts
        });

        if (response.Response != Responses.Success)
            return new(response.Response);

        response.Token = token;
        return response;

    }

}