namespace LIN.Identity.Areas.Accounts;


[Route("account")]
public class AccountController : ControllerBase
{

    /// <summary>
    /// Crear una cuenta.
    /// </summary>
    /// <param name="modelo">Modelo de la cuenta.</param>
    [HttpPost("create")]
    public async Task<HttpCreateResponse> Create([FromBody] AccountModel? modelo)
    {

        // Comprobaciones
        if (modelo == null || modelo.Identity == null || modelo.Contraseña.Length < 4 || modelo.Nombre.Length <= 0 || modelo.Identity.Unique.Length <= 0)
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son inválidos."
            };

        // Organización del modelo
        modelo = Account.Process(modelo);

        // Creación del usuario
        var response = await Data.Accounts.Create(modelo);

        // Evaluación
        if (response.Response != Responses.Success)
            return new(response.Response)
            {
                Message = "Hubo un error al crear la cuenta."
            };

        // Obtiene el usuario
        var token = Jwt.Generate(response.Model, 0);

        // Retorna el resultado
        return new CreateResponse()
        {
            LastID = response.Model.Identity.Id,
            Response = Responses.Success,
            Token = token,
            Message = "Cuenta creada satisfactoriamente."
        };

    }



   /// <summary>
   /// Obtener una cuenta.
   /// </summary>
   /// <param name="id">Id de la cuenta.</param>
   /// <param name="token">Token de acceso.</param>
    [HttpGet("read/id")]
    public async Task<HttpReadOneResponse<AccountModel>> Read([FromQuery] int id, [FromHeader] string token)
    {

        // Id es invalido.
        if (id <= 0 || string.IsNullOrWhiteSpace(token))
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son inválidos."
            };

        // Información del token.
        var (isValid, _, user, orgId, _) = Jwt.Validate(token);

        // Token es invalido.
        if (!isValid)
            return new ReadOneResponse<AccountModel>()
            {
                Response = Responses.Unauthorized,
                Message = "Token invalido."
            };

        // Obtiene el usuario.
        var response = await Data.Accounts.Read(id, new()
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
                Response = response.Response
            };

        // Retorna el resultado
        return response;

    }



    /// <summary>
    /// Obtener una cuenta.
    /// </summary>
    /// <param name="user">Identidad.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpGet("read/user")]
    public async Task<HttpReadOneResponse<AccountModel>> Read([FromQuery] string user, [FromHeader] string token)
    {

        // Usuario es invalido.
        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(token))
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son inválidos."
            };

        // Información del token.
        var (isValid, _, userId, orgId, _) = Jwt.Validate(token);

        // Token es invalido.
        if (!isValid)
            return new()
            {
                Response = Responses.Unauthorized,
                Message = "Token invalido."
            };

        // Obtiene el usuario.
        var response = await Data.Accounts.Read(user, new()
        {
            ContextOrg = orgId,
            ContextUser = userId,
            FindOn = FilterModels.FindOn.StableAccounts,
            IncludeOrg = FilterModels.IncludeOrg.IncludeIf,
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
    /// Obtiene una lista de diez (10) usuarios que coincidan con un patron
    /// </summary>
    /// <param name="pattern">Patron</param>
    /// <param name="token">Token de acceso</param>
    [HttpGet("search")]
    public async Task<HttpReadAllResponse<AccountModel>> Search([FromQuery] string pattern, [FromHeader] string token)
    {

        // Comprobación
        if (pattern.Trim().Length <= 0 || string.IsNullOrWhiteSpace(pattern) || string.IsNullOrWhiteSpace(token))
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son inválidos."
            };

        // Info del token
        var (isValid, _, userId, orgId, _) = Jwt.Validate(token);

        // Token es invalido
        if (!isValid)
            return new ReadAllResponse<AccountModel>
            {
                Message = "Token es invalido",
                Response = Responses.Unauthorized
            };

        // Obtiene el usuario
        var response = await Data.Accounts.Search(pattern, new()
        {
            ContextOrg = orgId,
            ContextUser = userId,
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

        // Comprobación
        if (string.IsNullOrWhiteSpace(token))
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son inválidos."
            };


        // Información del token.
        var (isValid, _, userId, orgId, _) = Jwt.Validate(token);

        // Es invalido.
        if (!isValid)
            return new()
            {
                Response = Responses.Unauthorized,
                Message = "Token invalido."
            };

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
    /// (ADMIN) encuentra diez (10) usuarios que coincidan con el patron
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="token"></param>
    [HttpGet("admin/search")]
    public async Task<HttpReadAllResponse<AccountModel>> FindAll([FromQuery] string pattern, [FromHeader] string token)
    {

        var (isValid, _, _, _, _) = Jwt.Validate(token);


        if (!isValid)
        {
            return new(Responses.Unauthorized);
        }


        var rol = AccountRoles.User;


        if (rol != AccountRoles.Admin)
            return new(Responses.Unauthorized);

        // Obtiene el usuario
        var response = await Data.Accounts.Search(pattern, new()
        {
            ContextOrg = 0,
            OrgLevel = FilterModels.IncludeOrgLevel.Advance,
            ContextUser = 0,
            FindOn = FilterModels.FindOn.AllAccount,
            IncludeOrg = FilterModels.IncludeOrg.Include,
            IsAdmin = true
        });

        return response;

    }



    /// <summary>
    /// Actualiza la información de una cuenta.
    /// </summary>
    /// <param name="modelo">Modelo</param>
    /// <param name="token">Token de acceso</param>
    [HttpPut("update")]
    public async Task<HttpResponseBase> Update([FromBody] AccountModel modelo, [FromHeader] string token)
    {

        // Información del token.
        var (isValid, _, userId, _, _) = Jwt.Validate(token);

        // Es invalido.
        if (!isValid)
            return new ResponseBase
            {
                Response = Responses.Unauthorized,
                Message = "Token Invalido"
            };

        // Organizar el modelo.
        modelo.Identity.Id = userId;
        modelo.Perfil = Image.Zip(modelo.Perfil ?? []);

        if (modelo.Identity.Id <= 0 || modelo.Nombre.Any())
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

        // Información del token.
        var (isValid, _, id, _, _) = Jwt.Validate(token);

        // Token es invalido.
        if (!isValid)
            return new()
            {
                Response = Responses.Unauthorized,
                Message = "Token invalido."
            };

        // Realizar actualización.
        return await Data.Accounts.Update(id, genero);

    }



    /// <summary>
    /// Actualiza la visibilidad de una cuenta.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="visibility">Nueva visibilidad.</param>
    [HttpPatch("update/visibility")]
    public async Task<HttpResponseBase> Update([FromHeader] string token, [FromHeader] AccountVisibility visibility)
    {

        // Información del token.
        var (isValid, _, id, _, _) = Jwt.Validate(token);

        // Token es invalido.
        if (!isValid)
            return new()
            {
                Response = Responses.Unauthorized,
                Message = "Token invalido."
            };

        // Actualización.
        return await Data.Accounts.Update(id, visibility);

    }



}