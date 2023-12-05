namespace LIN.Identity.Areas.V3;


[Route("v3/administrator")]
public class AdminController : ControllerBase
{



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


        var (isValid, _, user, orgID, _) = Jwt.Validate(token);

        if (!isValid)
        {
            return new ReadOneResponse<AccountModel>()
            {
                Response = Responses.Unauthorized,
                Message = "Token invalido."
            };
        }

        var rol = (await Data.Accounts.Read(user, new()
        {
            IncludeOrg = FilterModels.IncludeOrg.None,
            FindOn = FilterModels.FindOn.StableAccounts
        })).Model.Rol;

        if (rol != AccountRoles.Admin)
        {
            return new ReadOneResponse<AccountModel>()
            {
                Response = Responses.Unauthorized,
                Message = "Tienes que ser un administrador."
            };
        }


        // Obtiene el usuario
        var response = await Data.Accounts.Read(id, new()
        {
            SensibleInfo = false,
            ContextOrg = orgID,
            ContextUser = user,
            IsAdmin = true,
            FindOn = FilterModels.FindOn.StableAccounts,
            IncludeOrg = FilterModels.IncludeOrg.Include,
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

        // Validar el parámetro.
        if (string.IsNullOrWhiteSpace(user))
            return new(Responses.InvalidParam);

        // Información del token.
        var (isValid, _, userId, orgId, _) = Jwt.Validate(token);

        // Token es invalido.
        if (!isValid)
            return new ReadOneResponse<AccountModel>()
            {
                Response = Responses.Unauthorized,
                Message = "Token invalido."
            };



        var rol = (await Data.Accounts.Read(userId, new()
        {
            IncludeOrg = FilterModels.IncludeOrg.None,
            FindOn = FilterModels.FindOn.StableAccounts
        })).Model.Rol;

        if (rol != AccountRoles.Admin)
            return new ReadOneResponse<AccountModel>()
            {
                Response = Responses.Unauthorized,
                Message = "Tienes que ser un administrador."
            };




        var response = await Data.Accounts.Read(user, new()
        {
            SensibleInfo = false,
            ContextOrg = orgId,
            IsAdmin = true,
            ContextUser = userId,
            FindOn = FilterModels.FindOn.StableAccounts,
            IncludeOrg = FilterModels.IncludeOrg.Include,
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
    /// Actualiza la contraseña de una cuenta
    /// </summary>
    /// <param name="modelo">Modelo de actualización</param>
    /// <param name="token">Token de acceso</param>
    [HttpPatch("update/password")]
    public async Task<HttpResponseBase> Update([FromBody] UpdatePasswordModel modelo, [FromHeader] string token)
    {

        if (modelo.OldPassword.Length < 4 || modelo.NewPassword.Length < 4)
            return new(Responses.InvalidParam);


        var (isValid, _, userId, _, _) = Jwt.Validate(token);


        if (!isValid)
        {
            return new(Responses.Unauthorized);
        }

        modelo.Account = userId;

        var actualData = await Data.Accounts.ReadBasic(modelo.Account);

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