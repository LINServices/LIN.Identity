namespace LIN.Auth.Controllers;


[Route("account")]
public class AccountController : ControllerBase
{


    /// <summary>
    /// Crea una cuenta
    /// </summary>
    /// <param name="modelo">Modelo del usuario</param>
    [HttpPost("create")]
    public async Task<HttpCreateResponse> Create([FromBody] AccountModel modelo)
    {

        // Comprobaciones
        if (modelo == null || modelo.Contraseña.Length < 4 || modelo.Nombre.Length <= 0 || modelo.Usuario.Length <= 0)
            return new(Responses.InvalidParam);


        // Organización del modelo
        modelo.ID = 0;
        modelo.Contraseña = EncryptClass.Encrypt(Conexión.SecreteWord + modelo.Contraseña);
        modelo.Creación = DateTime.Now;
        modelo.Estado = AccountStatus.Normal;
        modelo.Insignia = AccountBadges.None;
        modelo.Rol = AccountRoles.User;
        modelo.Perfil = modelo.Perfil.Length == 0
                               ? System.IO.File.ReadAllBytes("wwwroot/profile.png")
                               : modelo.Perfil;

        // IA Nombre (Genero)
        try
        {
            if (modelo.Genero == Genders.Undefined)
            {
                // Consulta
                //var sex = await Developers.IAName(modelo.Nombre.Trim().Split(" ")[0]);

                // Manejo
               // modelo.Sexo = sex.Model;
            }
        }
        catch
        {
        }

        // Conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        // Creación del usuario
        var response = await Data.Users.Create(modelo, context);

        // Evaluación
        if (response.Response != Responses.Success)
            return new(response.Response);

        context.CloseActions(connectionKey);

        // Genera el token

        // Retorna el resultado
        return response ?? new();

    }



    /// <summary>
    /// Obtiene un usuario por medio del ID
    /// </summary>
    /// <param name="id">ID del usuario</param>
    [HttpGet("read/id")]
    public async Task<HttpReadOneResponse<AccountModel>> ReadOneByID([FromQuery] int id)
    {

        if (id <= 0)
            return new(Responses.InvalidParam);

        // Obtiene el usuario
        var response = await Data.Users.Read(id, true);

        // Si es erróneo
        if (response.Response != Responses.Success)
            return new ReadOneResponse<AccountModel>()
            {
                Response = response.Response,
                Model = new()
            };

        // Retorna el resultado
        return response;

    }



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



    /// <summary>
    /// Obtiene un usuario por medio de el usuario Unico
    /// </summary>
    /// <param name="user">Usuario</param>
    [HttpGet("read/user")]
    public async Task<HttpReadOneResponse<AccountModel>> ReadOneByUser([FromQuery] string user)
    {

        if (!user.Any())
            return new(Responses.InvalidParam);

        // Obtiene el usuario
        var response = await Data.Users.Read(user, true);

        // Si es erróneo
        if (response.Response != Responses.Success)
            return new ReadOneResponse<AccountModel>()
            {
                Response = response.Response,
                Model = new()
            };

        // Retorna el resultado
        return response;

    }



    /// <summary>
    /// Obtiene una lista de 10 usuarios cullo usuario cumpla con un patron
    /// </summary>
    /// <param name="pattern">Patron de búsqueda</param>
    /// <param name="id">ID del usuario que esta buscando</param>
    [HttpGet("searchByPattern")]
    public async Task<HttpReadAllResponse<AccountModel>> ReadAllSearch([FromHeader] string pattern, [FromHeader] int id)
    {

        // Comprobación
        if (id <= 0 || pattern.Trim().Length <= 0)
            return new(Responses.InvalidParam);


        // Obtiene el usuario
        var response = await Data.Users.SearchByPattern(pattern, id);

        return response;
    }









    /// <summary>
    /// Obtiene usuarios
    /// </summary>
    /// <param name="ids">Lista de IDs de los usuarios</param>
    [HttpPost("find")]
    public async Task<HttpReadAllResponse<AccountModel>> ReadAll([FromBody] List<int> ids)
    {

        // Obtiene el usuario
        var response = await Data.Users.FindAll(ids);

        return response;

    }











    /// <summary>
    /// Obtiene una lista de 5 usuarios cullo usuario cumpla con un patron (Solo admins)
    /// </summary>
    /// <param name="pattern">Patron de búsqueda</param>
    /// <param name="id">ID del usuario que esta buscando</param>
    [HttpGet("findAllUsers")]
    public async Task<HttpReadAllResponse<AccountModel>> ReadAllSearch([FromHeader] string pattern, [FromHeader] string token)
    {

        var (isValid, _, id) = Jwt.Validate(token);


        if (!isValid)
        {
            return new(Responses.Unauthorized);
        }


        var rol = (await Data.Users.Read(id, false)).Model.Rol;


        if (rol != AccountRoles.Admin)
            return new(Responses.InvalidParam);

        // Obtiene el usuario
        var response = await Data.Users.GetAll(pattern);

        return response;

    }



    /// <summary>
    /// Actualiza los datos de un usuario
    /// </summary>
    /// <param name="modelo">Nuevo modelo</param>
    [HttpPut("update")]
    public async Task<HttpResponseBase> Update([FromBody] AccountModel modelo, [FromHeader] string token)
    {

        var (isValid, _, userID) = Jwt.Validate(token);

        if (!isValid)
            return new ResponseBase
            {
                Response = Responses.Unauthorized,
                Message = "Token Invalido"
            };

        modelo.ID = userID;

        if (modelo.ID <= 0 || modelo.Nombre.Any())
            return new(Responses.InvalidParam);

        return await Data.Users.Update(modelo);

    }



    /// <summary>
    /// Actualiza la contraseña
    /// </summary>
    /// <param name="modelo">Nuevo modelo</param>
    [HttpPatch("update/password")]
    public async Task<HttpResponseBase> Update([FromBody] UpdatePasswordModel modelo)
    {

        if (modelo.Account <= 0 || modelo.OldPassword.Length < 4 || modelo.NewPassword.Length < 4)
            return new(Responses.InvalidParam);


        var actualData = await Data.Users.Read(modelo.Account, true);

        if (actualData.Response != Responses.Success)
            return new(Responses.NotExistAccount);

        var oldEncrypted = actualData.Model.Contraseña;


        if (oldEncrypted != actualData.Model.Contraseña)
        {
            return new ResponseBase(Responses.InvalidPassword);
        }

        return await Data.Users.UpdatePassword(modelo);

    }



    /// <summary>
    /// Elimina una cuenta
    /// </summary>
    /// <param name="id">ID del usuario</param>
    [HttpDelete("delete")]
    public async Task<HttpResponseBase> Delete([FromHeader] string token)
    {

        var (isValid, _, userID) = Jwt.Validate(token);

        if (!isValid)
            return new ResponseBase
            {
                Response = Responses.Unauthorized,
                Message = "Token invalido"
            };

        if (userID <= 0)
            return new(Responses.InvalidParam);

        var response = await Data.Users.Delete(userID);
        return response;
    }



    /// <summary>
    /// Actualiza los datos de un usuario
    /// </summary>
    /// <param name="modelo">Nuevo modelo</param>
    [HttpPatch("disable/account")]
    public async Task<HttpResponseBase> Disable([FromBody] AccountModel user)
    {

        if (user.ID <= 0)
        {
            return new(Responses.ExistAccount);
        }

        // Modelo de usuario de la BD
        var userModel = await Data.Users.Read(user.ID, false);

        if (userModel.Model.Contraseña != EncryptClass.Encrypt(Conexión.SecreteWord + user.Contraseña))
        {
            return new(Responses.InvalidPassword);
        }


        return await Data.Users.UpdateState(user.ID, AccountStatus.Disable);

    }



    /// <summary>
    /// Actualiza el genero de un usuario
    /// </summary>
    /// <param name="token">Token de acceso</param>
    /// <param name="genero">Nuevo genero</param>
    [HttpPatch("update/gender")]
    public async Task<HttpResponseBase> UpdateGender([FromHeader] string token, [FromHeader] Genders genero)
    {


        var (isValid, _, id) = Jwt.Validate(token);


        if (!isValid)
        {
            return new(Responses.Unauthorized);
        }

        return await Data.Users.UpdateGender(id, genero);

    }



    /// <summary>
    /// Obtiene los mails asociados a una cuenta
    /// </summary>
    /// <param name="token">Token de acceso</param>
    [HttpGet("mails")]
    public async Task<HttpReadAllResponse<EmailModel>> GetMails([FromHeader] string token)
    {

        var (isValid, _, id) = Jwt.Validate(token);

        if (!isValid)
        {
            return new(Responses.Unauthorized);
        }

        return await Data.Mails.ReadAll(id);

    }



}