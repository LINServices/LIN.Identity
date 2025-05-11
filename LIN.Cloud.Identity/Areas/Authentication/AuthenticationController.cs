using LIN.Cloud.Identity.Persistence.Repositories;
using LIN.Cloud.Identity.Services.Auth.Interfaces;

namespace LIN.Cloud.Identity.Areas.Authentication;

[Route("[controller]")]
public class AuthenticationController(IAuthentication authentication, IAccountRepository accountData, IPolicyRepository policyData) : AuthenticationBaseController
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
        authentication.SetCredentials(user, password, application);

        // Respuesta.
        var response = await authentication.Start(new()
        {
            ValidateApp = true,
            Log = true
        });

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

            // Contraseña invalida.
            case Responses.UnauthorizedByApp:
                return new()
                {
                    Response = Responses.UnauthorizedByApp,
                    Message = "La aplicación no existe o no permite que inicies sesión en este momento."
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
    /// Iniciar sesión y validar una política.
    /// </summary>
    /// <param name="user">Usuario.</param>
    /// <param name="password">Contraseña.</param>
    /// <param name="policy">Id de la política.</param>
    [HttpGet("validate/policy")]
    public async Task<HttpResponseBase> ValidatePolicy([FromQuery] string user, [FromQuery] string password, [FromHeader] int policy)
    {

        // Validación de parámetros.
        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son invalido."
            };

        // Establecer credenciales.
        authentication.SetCredentials(user, password, string.Empty);

        // Respuesta.
        var response = await authentication.Start(new()
        {
            Log = false
        });

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
                    Response = Responses.Unauthorized,
                    Message = "No existe esta cuenta."
                };

            // Contraseña invalida.
            case Responses.InvalidPassword:
                return new()
                {
                    Response = Responses.Unauthorized,
                    Message = "Contraseña incorrecta."
                };

            // Incorrecto
            default:
                return new()
                {
                    Response = Responses.Unauthorized,
                    Message = "Hubo un error grave."
                };
        }

        //// Validar política.
        //var isAllow = await policyData.HasFor(authentication.GetData().IdentityId, policy);

        //// Respuesta.
        //var http = new ResponseBase
        //{
        //    Response = isAllow.Response
        //};

        //return http;
        return new()
        {
            Response = Responses.Success
        };
    }

}