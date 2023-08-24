using System.Reflection.Metadata;

namespace LIN.Auth.Controllers;


[Route("account")]
public class AccountController : ControllerBase
{


    /// <summary>
    /// Crea un nuevo usuario
    /// </summary>
    /// <param name="modelo">Modelo</param>
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


        // Conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        // Creación del usuario
        var response = await Data.Accounts.Create(modelo, context);

        // Evaluación
        if (response.Response != Responses.Success)
            return new(response.Response);

        context.CloseActions(connectionKey);

        // Obtiene el usuario
        string token = Jwt.Generate(response.Model);


        // Retorna el resultado
        return new CreateResponse()
        {
            LastID = response.Model.ID,
            Response = Responses.Success,
            Token = token,
            Message = "Success"
        };

    }




    /// <summary>
    /// Obtiene un usuario por medio del ID
    /// </summary>
    /// <param name="id">ID del usuario</param>
    [HttpGet("read/id")]
    public async Task<HttpReadOneResponse<AccountModel>> Read([FromQuery] int id)
    {

        if (id <= 0)
            return new(Responses.InvalidParam);

        // Obtiene el usuario
        var response = await Data.Accounts.Read(id, true, false);

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
    /// Obtiene un usuario por medio del usuario único
    /// </summary>
    /// <param name="user">Usuario único</param>
    [HttpGet("read/user")]
    public async Task<HttpReadOneResponse<AccountModel>> Read([FromQuery] string user)
    {

        if (!user.Any())
            return new(Responses.InvalidParam);

        // Obtiene el usuario
        var response = await Data.Accounts.Read(user, true, false);

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
    /// Obtiene una lista de diez (10) usuarios que coincidan con un patron
    /// </summary>
    /// <param name="pattern">Patron</param>
    /// <param name="token">Token de acceso</param>
    [HttpGet("search")]
    public async Task<HttpReadAllResponse<AccountModel>> Search([FromHeader] string pattern, [FromHeader] string token)
    {

        // Comprobación
        if (pattern.Trim().Length <= 0)
            return new(Responses.InvalidParam);

        // Info del token
        var (isValid, _, userID, _) = Jwt.Validate(token);

        // Token es invalido
        if (!isValid)
        {
            return new ReadAllResponse<AccountModel>
            {
                Message = "Token es invalido",
                Response = Responses.Unauthorized
            };
        }

        // Obtiene el usuario
        var response = await Data.Accounts.Search(pattern, userID, false);

        return response;
    }




    /// <summary>
    /// Obtiene una lista cuentas
    /// </summary>
    /// <param name="ids">IDs de las cuentas</param>
    [HttpPost("findAll")]
    public async Task<HttpReadAllResponse<AccountModel>> ReadAll([FromBody] List<int> ids)
    {

        // Obtiene el usuario
        var response = await Data.Accounts.FindAll(ids);

        return response;

    }



    
    /// <summary>
    /// Actualiza la contraseña de una cuenta
    /// </summary>
    /// <param name="modelo">Modelo de actualización</param>
    [HttpPatch("update/password")]
    public async Task<HttpResponseBase> Update([FromBody] UpdatePasswordModel modelo, [FromHeader] string token)
    {

        if ( modelo.OldPassword.Length < 4 || modelo.NewPassword.Length < 4)
            return new(Responses.InvalidParam);


        var (isValid, _, userID, _) = Jwt.Validate(token);


        if (!isValid)
        {
            return new(Responses.Unauthorized);
        }

        modelo.Account = userID;

        var actualData = await Data.Accounts.Read(modelo.Account, true);

        if (actualData.Response != Responses.Success)
            return new(Responses.NotExistAccount);

        var oldEncrypted = actualData.Model.Contraseña;


        if (oldEncrypted != actualData.Model.Contraseña)
        {
            return new ResponseBase(Responses.InvalidPassword);
        }

        return await Data.Accounts.Update(modelo);

    }




    /// <summary>
    /// Elimina una cuenta
    /// </summary>
    /// <param name="token">Token de acceso</param>
    [HttpDelete("delete")]
    public async Task<HttpResponseBase> Delete([FromHeader] string token)
    {

        var (isValid, _, userID, _) = Jwt.Validate(token);

        if (!isValid)
            return new ResponseBase
            {
                Response = Responses.Unauthorized,
                Message = "Token invalido"
            };

        if (userID <= 0)
            return new(Responses.InvalidParam);

        var response = await Data.Accounts.Delete(userID);
        return response;
    }




    /// <summary>
    /// Desactiva una cuenta
    /// </summary>
    /// <param name="user">Modelo</param>
    [HttpPatch("disable")]
    public async Task<HttpResponseBase> Disable([FromBody] AccountModel user)
    {

        if (user.ID <= 0)
        {
            return new(Responses.ExistAccount);
        }

        // Modelo de usuario de la BD
        var userModel = await Data.Accounts.Read(user.ID,true);

        if (userModel.Model.Contraseña != EncryptClass.Encrypt(Conexión.SecreteWord + user.Contraseña))
        {
            return new(Responses.InvalidPassword);
        }


        return await Data.Accounts.Update(user.ID, AccountStatus.Disable);

    }




    /// <summary>
    /// (ADMIN) encuentra diez (10) usuarios que coincidan con el patron
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="token"></param>
    [HttpGet("admin/search")]
    public async Task<HttpReadAllResponse<AccountModel>> FindAll([FromQuery] string pattern, [FromHeader] string token)
    {

        var (isValid, _, id, _) = Jwt.Validate(token);


        if (!isValid)
        {
            return new(Responses.Unauthorized);
        }


        var rol = (await Data.Accounts.Read(id, true)).Model.Rol;


        if (rol != AccountRoles.Admin)
            return new(Responses.Unauthorized);

        // Obtiene el usuario
        var response = await Data.Accounts.Search(pattern, 0, true);

        return response;

    }




    /// <summary>
    /// Actualiza la información de una cuenta
    /// </summary>
    /// <param name="modelo">Modelo</param>
    /// <param name="token">Token de acceso</param>
    [HttpPut("update")]
    public async Task<HttpResponseBase> Update([FromBody] AccountModel modelo, [FromHeader] string token)
    {

        var (isValid, _, userID, _) = Jwt.Validate(token);

        if (!isValid)
            return new ResponseBase
            {
                Response = Responses.Unauthorized,
                Message = "Token Invalido"
            };

        modelo.ID = userID;

        if (modelo.ID <= 0 || modelo.Nombre.Any())
            return new(Responses.InvalidParam);

        return await Data.Accounts.Update(modelo);

    }




    /// <summary>
    /// Actualiza el genero de un usuario
    /// </summary>
    /// <param name="token">Token de acceso</param>
    /// <param name="genero">Nuevo genero</param>
    [HttpPatch("update/gender")]
    public async Task<HttpResponseBase> Update([FromHeader] string token, [FromHeader] Genders genero)
    {


        var (isValid, _, id, _) = Jwt.Validate(token);


        if (!isValid)
        {
            return new(Responses.Unauthorized);
        }

        return await Data.Accounts.Update(id, genero);

    }




    /// <summary>
    /// Actualiza la visibilidad de una cuenta
    /// </summary>
    /// <param name="token">Token de acceso</param>
    /// <param name="visibility">Nueva visibilidad</param>
    [HttpPatch("update/visibility")]
    public async Task<HttpResponseBase> Update([FromHeader] string token, [FromHeader] AccountVisibility visibility)
    {


        var (isValid, _, id, _) = Jwt.Validate(token);

        if (!isValid)
        {
            return new(Responses.Unauthorized);
        }

        return await Data.Accounts.Update(id, visibility);

    }



}