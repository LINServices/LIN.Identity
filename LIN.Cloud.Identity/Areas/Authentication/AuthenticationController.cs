namespace LIN.Cloud.Identity.Areas.Authentication;

[Route("[controller]")]
public class AuthenticationController(IAuthenticationAccountService serviceAuth, IAccountRepository accountData) : AuthenticationBaseController
{

    /// <summary>
    /// Iniciar sesión en una cuenta de usuario.
    /// </summary>
    /// <param name="user">Unique.</param>
    /// <param name="password">Contraseña.</param>
    /// <param name="application">Id de la aplicación.</param>
    /// <returns>Retorna el modelo de la cuenta y el token de acceso.</returns>
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
        var response = await serviceAuth.Authenticate(new()
        {
            User = user,
            Password = password,
            Application = application,
            AuthenticationMethod = AuthenticationMethods.Password
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
        var token = serviceAuth.GenerateToken();

        // Respuesta.
        var http = new ReadOneResponse<AccountModel>
        {
            Model = serviceAuth.Account!,
            Response = Responses.Success,
            Alternatives = [.. serviceAuth.Identities.Select(t=>(object)t)],
            Token = token
        };

        return http;
    }


    /// <summary>
    /// Refrescar sesión en una cuenta de usuario.
    /// </summary>
    /// <returns>Retorna el modelo de la cuenta.</returns>
    [HttpGet("LoginWithToken")]
    [IdentityToken]
    public async Task<HttpReadOneResponse<AccountModel>> LoginWithToken()
    {
        // Obtiene el usuario.
        var response = await accountData.Read(UserInformation.AccountId, new QueryObjectFilter()
        {
            IsAdmin = true,
            FindOn = FindOn.StableAccounts
        });

        if (response.Response != Responses.Success)
            return new(response.Response);

        response.Token = Token;
        return response;
    }


    /// <summary>
    /// Iniciar sesión con un tercero.
    /// </summary>
    /// <param name="token">Token de acceso a tercero.</param>
    [HttpGet("ThirdParty")]
    public async Task<HttpReadOneResponse<AccountModel>> LoginWith([FromHeader] string token, [FromHeader] IdentityService provider, [FromHeader] string application)
    {

        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(application))
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son invalido."
            };

        // Validar información del token.
        var request = new AuthenticationRequest
        {
            ThirdPartyToken = token,
            Service = provider,
            StrictService = true,
            Application = application
        };

        var response = await serviceAuth.Authenticate(request);

        if (response.Response != Responses.Success)
            return new()
            {
                Response = response.Response,
                Message = response.Message,
                Errors = response.Errors
            };

        // Respuesta.
        var http = new ReadOneResponse<AccountModel>
        {
            Model = serviceAuth.Account!,
            Response = Responses.Success,
            Token = serviceAuth.GenerateToken()
        };

        return http;
    }

}