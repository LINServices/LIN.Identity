namespace LIN.Cloud.Identity.Areas.Accounts;

[Route("[controller]")]
public class AccountController(Data.Accounts accountData, Data.Applications applications) : AuthenticationBaseController
{

    /// <summary>
    /// Crear cuenta de enlace LIN.
    /// </summary>
    /// <param name="modelo">Modelo de la cuenta.</param>
    /// <returns>Retorna el Id asignado a la cuenta.</returns>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromBody] AccountModel? modelo, [FromHeader] string app)
    {

        // Validaciones del modelo.
        if (modelo is null || modelo.Identity is null || modelo.Password.Length < 4 || modelo.Name.Length <= 0 || modelo.Identity.Unique.Length <= 0)
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son inválidos."
            };

        // Validar usuario y nombre.
        var errors = Services.Formats.Account.Validate(modelo);

        // Si no fue valido.
        if (errors.Count > 0)
            return new(Responses.InvalidParam)
            {
                Message = "Ocurrió un error al crear la cuenta",
                Errors = errors
            };

        // Organización del modelo.
        modelo.AccountType = AccountTypes.Personal;
        modelo = Services.Formats.Account.Process(modelo);

        // Creación del usuario.
        var response = await accountData.Create(modelo);

        // Evaluación.
        if (response.Response != Responses.Success)
            return new(response.Response)
            {
                Message = "Hubo un error al crear la cuenta."
            };

        // Obtener información de la app.
        var application = await applications.Read(app);

        // Obtiene el usuario.
        string? token = null;

        // Si la aplicación es valida, generamos un token.
        if (application.Response == Responses.Success)
            token = JwtService.Generate(response.Model, application.Model.Id);
        
        // Retorna el resultado.
        return new CreateResponse()
        {
            LastId = response.Model.Identity.Id,
            Response = Responses.Success,
            Token = token,
            Message = "Cuenta creada satisfactoriamente."
        };

    }


    /// <summary>
    /// Obtener una cuenta.
    /// </summary>
    /// <param name="id">Id de la cuenta.</param>
    /// <returns>Retorna el modelo de la cuenta.</returns>
    [HttpGet("read/id")]
    [IdentityToken]
    public async Task<HttpReadOneResponse<AccountModel>> Read([FromQuery] int id)
    {

        // Id es invalido.
        if (id <= 0)
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son inválidos."
            };

        // Obtiene el usuario.
        var response = await accountData.Read(id, new()
        {
            AccountContext = UserInformation.AccountId,
            FindOn = FindOn.StableAccounts,
            IdentityContext = UserInformation.IdentityId,
            IsAdmin = false
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
    /// <param name="user">Unique de la identidad de la cuenta.</param>
    /// <returns>Retorna el modelo de la cuenta.</returns>
    [HttpGet("read/user")]
    [IdentityToken]
    public async Task<HttpReadOneResponse<AccountModel>> Read([FromQuery] string user)
    {

        // Usuario es invalido.
        if (string.IsNullOrWhiteSpace(user))
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son inválidos."
            };

        // Obtiene el usuario.
        var response = await accountData.Read(user, new()
        {
            AccountContext = UserInformation.AccountId,
            FindOn = FindOn.StableAccounts,
            IdentityContext = UserInformation.IdentityId,
            IsAdmin = false
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
    /// Obtener una cuenta.
    /// </summary>
    /// <param name="id">Id de la identidad.</param>
    /// <returns>Retorna el modelo de la cuenta.</returns>
    [HttpGet("read/identity")]
    [IdentityToken]
    public async Task<HttpReadOneResponse<AccountModel>> ReadByIdentity([FromQuery] int id)
    {

        // Id es invalido.
        if (id <= 0)
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son inválidos."
            };

        // Obtiene el usuario.
        var response = await accountData.ReadByIdentity(id, new()
        {
            AccountContext = UserInformation.AccountId,
            FindOn = FindOn.StableAccounts,
            IdentityContext = UserInformation.IdentityId,
            IsAdmin = false
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
    /// Obtener una lista de cuentas según las id de las identidades.
    /// </summary>
    /// <param name="ids">Id de las identidades</param>
    /// <returns>Retorna la lista de cuentas.</returns>
    [HttpPost("read/identity")]
    [IdentityToken]
    public async Task<HttpReadAllResponse<AccountModel>> ReadByIdentity([FromBody] List<int> ids)
    {
        // Obtiene el usuario
        var response = await accountData.FindAllByIdentities(ids, new()
        {
            AccountContext = UserInformation.AccountId,
            FindOn = FindOn.StableAccounts,
            IsAdmin = false,
            IdentityContext = UserInformation.IdentityId,
        });

        return response;
    }


    /// <summary>
    /// Buscar cuentas por medio de un patrón de búsqueda.
    /// </summary>
    /// <param name="pattern">Patron de búsqueda.</param>
    /// <returns>Retorna las cuentas encontradas.</returns>
    [HttpGet("search")]
    [IdentityToken]
    public async Task<HttpReadAllResponse<AccountModel>> Search([FromQuery] string pattern)
    {

        // Comprobación
        if (pattern.Trim().Length <= 0 || string.IsNullOrWhiteSpace(pattern))
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son inválidos."
            };

        // Obtiene el usuario
        var response = await accountData.Search(pattern, new()
        {
            AccountContext = UserInformation.AccountId,
            FindOn = FindOn.StableAccounts,
            IsAdmin = false,
            IdentityContext = UserInformation.IdentityId,
        });

        return response;
    }


    /// <summary>
    /// Obtener la lista de cuentas.
    /// </summary>
    /// <param name="ids">Lista de ids de las cuentas.</param>
    /// <returns>Retorna una lista de las cuentas encontradas.</returns>
    [HttpPost("findAll")]
    [IdentityToken]
    public async Task<HttpReadAllResponse<AccountModel>> ReadAll([FromBody] List<int> ids)
    {
        // Obtiene el usuario
        var response = await accountData.FindAll(ids, new()
        {
            AccountContext = UserInformation.AccountId,
            FindOn = FindOn.StableAccounts,
            IsAdmin = false,
            IdentityContext = UserInformation.IdentityId,
        });

        return response;

    }

}