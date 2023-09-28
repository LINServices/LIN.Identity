using LIN.Identity.Areas.Auth.Login;

namespace LIN.Identity.Controllers;


[Route("authentication")]
public class AuthenticationController : ControllerBase
{


    /// <summary>
    /// Inicia una sesión de usuario
    /// </summary>
    /// <param name="user">Usuario único</param>
    /// <param name="password">Contraseña del usuario</param>
    /// <param name="application">Key de aplicación</param>
    [HttpGet("login")]
    public async Task<HttpReadOneResponse<AccountModel>> Login([FromQuery] string user, [FromQuery] string password, [FromHeader] string application)
    {

        // Validación de parámetros.
        if (!user.Any() || !password.Any() || !application.Any())
            return new(Responses.InvalidParam);

        // Obtiene el usuario.
        var response = await Data.Accounts.Read(user, true);

        // Validación al obtener el usuario
        switch (response.Response)
        {
            // Correcto
            case Responses.Success:
                break;

            // Incorrecto
            default:
                return new(response.Response);
        }


        // Estrategia de login
        LoginBase strategy;

        // Definir la estrategia
        strategy = response.Model.OrganizationAccess == null ? new LoginNormal(response.Model, application, password)
            : new LoginOnOrg(response.Model, application, password);

        // Respuesta del login
        var loginResponse = await strategy.Login();


        // Respuesta
        if (loginResponse.Response != Responses.Success)
            return new ReadOneResponse<AccountModel>()
            {
                Message = loginResponse.Message,
                Response = loginResponse.Response
            };


        // Genera el token
        var token = Jwt.Generate(response.Model, 0);


        response.Token = token;
        return response;

    }



    /// <summary>
    /// Inicia una sesión de usuario por medio del token
    /// </summary>
    /// <param name="token">Token de acceso</param>
    [HttpGet("LoginWithToken")]
    public async Task<HttpReadOneResponse<AccountModel>> LoginWithToken([FromHeader] string token)
    {

        // Valida el token
        var (isValid, _, user, _, _) = Jwt.Validate(token);

        if (!isValid)
            return new(Responses.InvalidParam);


        // Obtiene el usuario
        var response = await Data.Accounts.Read(user, true);

        if (response.Response != Responses.Success)
            return new(response.Response);

        if (response.Model.Estado != AccountStatus.Normal)
            return new(Responses.NotExistAccount);

        response.Token = token;
        return response;

    }



}