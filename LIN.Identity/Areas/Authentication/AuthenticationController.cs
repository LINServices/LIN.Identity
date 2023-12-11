using LIN.Identity.Services.Login;

namespace LIN.Identity.Areas.Authentication;


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
        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(application))
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son invalido."
            };


        // Obtiene el usuario.
        var response = await Data.Accounts.Read(user, new()
        {
            SensibleInfo = true,
            IsAdmin = true,
            IncludeOrg = FilterModels.IncludeOrg.Include,
            OrgLevel = FilterModels.IncludeOrgLevel.Advance,
            FindOn = FilterModels.FindOn.StableAccounts
        });

        // Validación al obtener el usuario
        switch (response.Response)
        {
            // Correcto
            case Responses.Success:
                break;

            // Incorrecto
            default:
                return new(response.Response)
                {
                    Message = "Hubo un error grave."
                };
        }


        // Estrategia de login
        LoginService strategy;

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

        // Información del token de acceso.
        var (isValid, _, user, _, _) = Jwt.Validate(token);

        // Si el token es invalido.
        if (!isValid)
            return new(Responses.InvalidParam)
            {
                Message = "El token proporcionado no es valido."
            };

        // Obtiene el usuario
        var response = await Data.Accounts.Read(user, new()
        {
            SensibleInfo = true,
            IsAdmin = true,
            IncludeOrg = FilterModels.IncludeOrg.Include,
            OrgLevel = FilterModels.IncludeOrgLevel.Advance,
            FindOn = FilterModels.FindOn.StableAccounts
        });

        if (response.Response != Responses.Success)
            return new(response.Response);

        if (response.Model.Estado != AccountStatus.Normal)
            return new(Responses.NotExistAccount);

        response.Token = token;
        return response;

    }




    [HttpGet("Hi")]
    public async Task<dynamic> Get([FromHeader]int account)
    {
        var iam = await Services.Iam.Applications.ValidateAccess(account, 0);
        return iam;
    }
}