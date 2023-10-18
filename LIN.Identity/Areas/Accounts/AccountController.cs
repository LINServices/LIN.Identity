using LIN.Identity.Validations;
namespace LIN.Identity.Areas.Accounts;


[Route("account")]
public class AccountController : ControllerBase
{


    /// <summary>
    /// Crear nueva cuenta (Cuenta de LIN)
    /// </summary>
    /// <param name="modelo">Modelo de la cuenta</param>
    [HttpPost("create")]
    public async Task<HttpCreateResponse> Create([FromBody] AccountModel? modelo)
    {

        // Comprobaciones
        if (modelo == null || modelo.Contraseña.Length < 4 || modelo.Nombre.Length <= 0 || modelo.Usuario.Length <= 0)
            return new(Responses.InvalidParam);

        // Organización del modelo
        modelo = Account.Process(modelo);

        // Creación del usuario
        var response = await Data.Accounts.Create(modelo);

        // Evaluación
        if (response.Response != Responses.Success)
            return new(response.Response);

        // Obtiene el usuario
        var token = Jwt.Generate(response.Model, 0);

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
    /// Obtiene la información de usuario.
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="token">Token de acceso</param>
    [HttpGet("read/id")]
    public async Task<HttpReadOneResponse<AccountModel>> Read([FromQuery] int id, [FromHeader] string token)
    {

        if (id <= 0)
            return new(Responses.InvalidParam);

        var (isValid, _, user, orgId, _) = Jwt.Validate(token);

        if (!isValid)
        {
            return new ReadOneResponse<AccountModel>()
            {
                Response = Responses.Unauthorized,
                Message = "Token invalido."
            };
        }

        // Obtiene el usuario
        var response = await Data.Accounts.Read(id, new FilterModels.Account()
        {
            ContextOrg = orgId,
            ContextUser = user,
            FindOn = FilterModels.FindOn.StableAccounts,
            IncludeOrg = FilterModels.IncludeOrg.IncludeIf,
            IsAdmin = false,
            OrgLevel = FilterModels.IncludeOrgLevel.Advance
        });

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
    /// Obtiene la información de usuario.
    /// </summary>
    /// <param name="user">Usuario único</param>
    /// <param name="token">Token de acceso</param>
    [HttpGet("read/user")]
    public async Task<HttpReadOneResponse<AccountModel>> Read([FromQuery] string user, [FromHeader] string token)
    {

        if (string.IsNullOrWhiteSpace(user))
            return new(Responses.InvalidParam);


        var (isValid, _, userId, orgId, _) = Jwt.Validate(token);

        if (!isValid)
        {
            return new ReadOneResponse<AccountModel>()
            {
                Response = Responses.Unauthorized,
                Message = "Token invalido."
            };
        }



        var response = await Data.Accounts.Read(user, new FilterModels.Account()
        {
            ContextOrg = orgId,
            ContextUser = userId,
            FindOn= FilterModels.FindOn.StableAccounts,
            IncludeOrg= FilterModels.IncludeOrg.IncludeIf,
            OrgLevel= FilterModels.IncludeOrgLevel.Advance
        });



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
    public async Task<HttpReadAllResponse<AccountModel>> Search([FromQuery] string pattern, [FromHeader] string token)
    {

        // Comprobación
        if (pattern.Trim().Length <= 0)
            return new(Responses.InvalidParam);

        // Info del token
        var (isValid, _, userId, orgId, _) = Jwt.Validate(token);

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
        var response = await Data.Accounts.Search(pattern, new()
        {
            ContextOrg= orgId,
            ContextUser= userId,
            FindOn = FilterModels.FindOn.StableAccounts,
            IncludeOrg = FilterModels.IncludeOrg.IncludeIf,
            OrgLevel = FilterModels.IncludeOrgLevel.Advance
        });

        return response;
    }




    /// <summary>
    /// Obtiene una lista cuentas
    /// </summary>
    /// <param name="ids">IDs de las cuentas</param>
    /// <param name="token">Token de acceso</param>
    [HttpPost("findAll")]
    public async Task<HttpReadAllResponse<AccountModel>> ReadAll([FromBody] List<int> ids, [FromHeader] string token)
    {

        var (isValid, _, userId, orgId, _) = Jwt.Validate(token);

        if (!isValid)
        {
            return new(Responses.Unauthorized);
        }

        // Obtiene el usuario
        var response = await Data.Accounts.FindAll(ids, new()
        {
            ContextOrg = orgId,
            ContextUser = userId,
            FindOn = FilterModels.FindOn.StableAccounts,
            IncludeOrg = FilterModels.IncludeOrg.Include,
            OrgLevel = FilterModels.IncludeOrgLevel.Advance
        });

        return response;

    }




    /// <summary>
    /// Actualiza la contraseña de una cuenta
    /// </summary>
    /// <param name="modelo">Modelo de actualización</param>
    /// <param name="token">Token de acceso</param>
    [HttpPatch("update/password")]
    public async Task<HttpResponseBase> Update([FromBody] UpdatePasswordModel modelo, [FromHeader] string token)
    {

        // Validar parámetros.
        if (modelo == null)
            return new ResponseBase()
            {
                Message = "Parámetro para nueva actualización de contraseña es invalido.",
                Response = Responses.InvalidParam
            };
        
        // Validar de la nueva contraseña.
        if (modelo.OldPassword.Length < 4 || modelo.NewPassword.Length < 4)
            return new ResponseBase(Responses.InvalidParam)
            {
                Message = "Antigua contraseña o nueva contraseña tienen una longitud invalida."
            };

        // Validar el token.
        var (isValid, _, userId, _, _) = Jwt.Validate(token);

        // No es valido.
        if (!isValid)
            return new ResponseBase(Responses.Unauthorized)
            {
                Message = "Token invalido"
            };

        // Obtener datos antiguos.
        var actualData = await Data.Accounts.ReadBasic(userId);

        // Error al encontrar usuario.
        if (actualData.Response != Responses.Success)
            return new ResponseBase(Responses.Unauthorized)
            {
                Message = $"Error al encontrar el usuario con ID '{userId}'"
            };
        

        // Encriptar la contraseña
        modelo.OldPassword = EncryptClass.Encrypt(modelo.OldPassword);
        
        // Valida la contraseña actual
        if (modelo.OldPassword != actualData.Model.Contraseña)
            return new ResponseBase(Responses.InvalidPassword)
            {
                Message = $"Las contraseñas no coinciden."
            };

        // Actualiza el ID.
        modelo.Account = userId;

        // Actualizar la contraseña
        return await Data.Accounts.Update(modelo);

    }




    /// <summary>
    /// Elimina una cuenta
    /// </summary>
    /// <param name="token">Token de acceso</param>
    [HttpDelete("delete")]
    public async Task<HttpResponseBase> Delete([FromHeader] string token)
    {

        var (isValid, _, userId, _, _) = Jwt.Validate(token);

        if (!isValid)
            return new ResponseBase
            {
                Response = Responses.Unauthorized,
                Message = "Token invalido"
            };

        if (userId <= 0)
            return new(Responses.InvalidParam);

        var response = await Data.Accounts.Delete(userId);
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
        var userModel = await Data.Accounts.ReadBasic(user.ID);

        if (userModel.Model.Contraseña != EncryptClass.Encrypt(user.Contraseña))
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

        var (isValid, _, id, _, _) = Jwt.Validate(token);


        if (!isValid)
        {
            return new(Responses.Unauthorized);
        }


        var rol = (await Data.Accounts.Read(id, new FilterModels.Account
        {
            FindOn = FilterModels.FindOn.StableAccounts,
            IncludeOrg = FilterModels.IncludeOrg.None
        })).Model.Rol;


        if (rol != AccountRoles.Admin)
            return new(Responses.Unauthorized);

        // Obtiene el usuario
        var response = await Data.Accounts.Search(pattern, new()
        {
            ContextOrg = 0,
            OrgLevel = FilterModels.IncludeOrgLevel.Advance,
            ContextUser =0,
            FindOn = FilterModels.FindOn.AllAccount,
            IncludeOrg = FilterModels.IncludeOrg.Include,
            IsAdmin = true,
        });

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

        var (isValid, _, userId, _, _) = Jwt.Validate(token);

        if (!isValid)
            return new ResponseBase
            {
                Response = Responses.Unauthorized,
                Message = "Token Invalido"
            };

        modelo.ID = userId;
        modelo.Perfil = Image.Zip(modelo.Perfil);

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


        var (isValid, _, id, _, _) = Jwt.Validate(token);


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


        var (isValid, _, id, _, _) = Jwt.Validate(token);

        if (!isValid)
        {
            return new(Responses.Unauthorized);
        }

        return await Data.Accounts.Update(id, visibility);

    }



}