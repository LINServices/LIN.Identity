using LIN.Auth.Data.Accounts;

namespace LIN.Auth.Controllers;


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

        // Obtiene la App.
        var app = await Data.Applications.Read(application);

        // Verifica si la app existe.
        if (app.Response != Responses.Success)
        {
            return new ReadOneResponse<AccountModel>
            {
                Message = "La aplicación no esta autorizada para iniciar sesión en LIN Identity",
                Response = Responses.Unauthorized
            };
        }


        // Obtiene el usuario.
        var response = await AccountsGet.Read(user, true, true, true);

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

        // Valida el estado de la cuenta
        if (response.Model.Estado != AccountStatus.Normal)
            return new(Responses.NotExistAccount);

        // Valida la contraseña
        if (response.Model.Contraseña != EncryptClass.Encrypt(Conexión.SecreteWord + password))
            return new(Responses.InvalidPassword);


        // Obtiene la organización
        var org = response.Model.OrganizationAccess;

        // Validaciones de la organización
        if (org != null)
        {

            if (!org.Organization.LoginAccess && !org.Rol.IsAdmin())
            {
                return new ReadOneResponse<AccountModel>
                {
                    Message = "Tu organización a deshabilitado el inicio de sesión temporalmente.",
                    Response = Responses.LoginBlockedByOrg
                };
            }



            if (org.Organization.HaveWhiteList)
            {
                var have = org.Organization.AppList.Where(T => T.App.Key == application).FirstOrDefault();

                if (have == null)
                {
                    return new ReadOneResponse<AccountModel>
                    {
                        Message = "Tu organización no permite iniciar sesión en esta aplicación.",
                        Response = Responses.UnauthorizedByOrg
                    };
                }
            }



        }


        // Genera el token
        var token = Jwt.Generate(response.Model);

        // Crea registro del login
        _ = Data.Logins.Create(new()
        {
            Date = DateTime.Now,
            AccountID = response.Model.ID,
            ApplicationID = app.Model.ID
        });

        if (response.Model.OrganizationAccess != null)
        {
            response.Model.OrganizationAccess.Organization.AppList = new();
            response.Model.OrganizationAccess.Organization.Members = new();
        }

        response.Token = token;
        return response;

    }



    /// <summary>
    /// Inicia una sesión de usuario por medio del token
    /// </summary>
    /// <param name="token">Token de acceso</param>
    [HttpGet("LoginWithToken")]
    public async Task<HttpReadOneResponse<AccountModel>> LoginWithToken([FromHeader] string token, [FromHeader] string application)
    {

        // Valida el token
        var (isValid, user, _, _) = Jwt.Validate(token);

        if (!isValid)
            return new(Responses.InvalidParam);


        // Obtiene el usuario
        var response = await AccountsGet.Read(user, true);

        if (response.Response != Responses.Success)
            return new(response.Response);

        if (response.Model.Estado != AccountStatus.Normal)
            return new(Responses.NotExistAccount);

        // Crea registro del login
        _ = Data.Logins.Create(new()
        {
            Date = DateTime.Now,
            AccountID = response.Model.ID
        });

        response.Token = token;
        return response;

    }



}