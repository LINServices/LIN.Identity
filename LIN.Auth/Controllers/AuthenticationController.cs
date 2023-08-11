namespace LIN.Auth.Controllers;


[Route("authentication")]
public class AuthenticationController : ControllerBase
{


    /// <summary>
    /// Inicia una sesión de usuario
    /// </summary>
    /// <param name="user">Usuario único</param>
    /// <param name="password">Contraseña del usuario</param>
    [HttpGet("login")]
    public async Task<HttpReadOneResponse<AccountModel>> Login([FromQuery] string user, [FromQuery] string password)
    {

        // Comprobación
        if (!user.Any() || !password.Any())
            return new(Responses.InvalidParam);


        // Obtiene el usuario
        var response = await Data.Users.Read(user, true);

        if (response.Response != Responses.Success)
            return new(response.Response);

        if (response.Model.Estado != AccountStatus.Normal)
            return new(Responses.NotExistAccount);

        if (response.Model.Contraseña != EncryptClass.Encrypt(Conexión.SecreteWord + password))
            return new(Responses.InvalidPassword);

        // Genera el token
        var token = Jwt.Generate(response.Model);

        // Crea registro del login
        _ = Data.Logins.Create(new()
        {
            Date = DateTime.Now,
            AccountID = response.Model.ID
        });

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
        (var isValid, var user, var _) = Jwt.Validate(token);

        if (!isValid)
            return new(Responses.InvalidParam);


        // Obtiene el usuario
        var response = await Data.Users.Read(user, true);

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