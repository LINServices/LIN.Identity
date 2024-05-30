namespace LIN.Cloud.Identity.Areas.Accounts;


[Route("[controller]")]
public class AccountController(Data.Accounts accountData) : ControllerBase
{


    /// <summary>
    /// Crear una cuenta LIN.
    /// </summary>
    /// <param name="modelo">Modelo de la cuenta.</param>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromBody] AccountModel? modelo)
    {

        // Validaciones del modelo.
        if (modelo == null || modelo.Identity == null || modelo.Password.Length < 4 || modelo.Name.Length <= 0 || modelo.Identity.Unique.Length <= 0)
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son inválidos."
            };

        // Validar usuario y nombre.
        var (pass, message) = Services.Formats.Account.Validate(modelo);

        // Si no fue valido.
        if (!pass)
            return new(Responses.InvalidParam)
            {
                Message = message
            };

        // Organización del modelo
        modelo = Services.Formats.Account.Process(modelo);

        // Creación del usuario
        var response = await accountData.Create(modelo, 0);

        // Evaluación
        if (response.Response != Responses.Success)
            return new(response.Response)
            {
                Message = "Hubo un error al crear la cuenta."
            };

        // Obtiene el usuario
        var token = JwtService.Generate(response.Model, 0);

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
    [IdentityToken]
    public async Task<HttpReadOneResponse<AccountModel>> Read([FromQuery] int id, [FromHeader] string token)
    {

        // Id es invalido.
        if (id <= 0)
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son inválidos."
            };

        // Token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

        // Obtiene el usuario.
        var response = await accountData.Read(id, new()
        {
            AccountContext = tokenInfo.AccountId,
            FindOn = FindOn.StableAccounts,
            IdentityContext = tokenInfo.IdentityId,
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
    /// <param name="user">Identidad única.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpGet("read/user")]
    [IdentityToken]
    public async Task<HttpReadOneResponse<AccountModel>> Read([FromQuery] string user, [FromHeader] string token)
    {

        // Usuario es invalido.
        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(token))
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son inválidos."
            };

        // Token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

        // Obtiene el usuario.
        var response = await accountData.Read(user, new()
        {
            AccountContext = tokenInfo.AccountId,
            FindOn = FindOn.StableAccounts,
            IdentityContext = tokenInfo.IdentityId,
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
    /// Obtener una cuenta según Id de identidad.
    /// </summary>
    /// <param name="id">Id de la identidad.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpGet("read/identity")]
    [IdentityToken]
    public async Task<HttpReadOneResponse<AccountModel>> ReadByIdentity([FromQuery] int id, [FromHeader] string token)
    {

        // Id es invalido.
        if (id <= 0)
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son inválidos."
            };

        // Token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

        // Obtiene el usuario.
        var response = await accountData.ReadByIdentity(id, new()
        {
            AccountContext = tokenInfo.AccountId,
            FindOn = FindOn.StableAccounts,
            IdentityContext = tokenInfo.IdentityId,
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
    /// Obtener una cuenta según Id de identidad.
    /// </summary>
    /// <param name="id">Id de la identidad.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpPost("read/identity")]
    [IdentityToken]
    public async Task<HttpReadAllResponse<AccountModel>> ReadByIdentity([FromBody] List<int> ids, [FromHeader] string token)
    {

        // Token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

        // Obtiene el usuario
        var response = await accountData.FindAllByIdentities(ids, new()
        {
            AccountContext = tokenInfo.AccountId,
            FindOn = FindOn.StableAccounts,
            IsAdmin = false,
            IdentityContext = tokenInfo.IdentityId,
        });

        return response;

    }



    /// <summary>
    /// Obtiene una lista de diez (10) usuarios que coincidan con un patron.
    /// </summary>
    /// <param name="pattern">Patron</param>
    /// <param name="token">Token de acceso</param>
    [HttpGet("search")]
    [IdentityToken]
    public async Task<HttpReadAllResponse<AccountModel>> Search([FromQuery] string pattern, [FromHeader] string token)
    {

        // Comprobación
        if (pattern.Trim().Length <= 0 || string.IsNullOrWhiteSpace(pattern))
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son inválidos."
            };

        // Token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();


        // Obtiene el usuario
        var response = await accountData.Search(pattern, new()
        {
            AccountContext = tokenInfo.AccountId,
            FindOn = FindOn.StableAccounts,
            IsAdmin = false,
            IdentityContext = tokenInfo.IdentityId,
        });

        return response;
    }



    /// <summary>
    /// Obtiene una lista cuentas.
    /// </summary>
    /// <param name="ids">IDs de las cuentas</param>
    /// <param name="token">Token de acceso</param>
    [HttpPost("findAll")]
    [IdentityToken]
    public async Task<HttpReadAllResponse<AccountModel>> ReadAll([FromBody] List<int> ids, [FromHeader] string token)
    {

        // Token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

        // Obtiene el usuario
        var response = await accountData.FindAll(ids, new()
        {
            AccountContext = tokenInfo.AccountId,
            FindOn = FindOn.StableAccounts,
            IsAdmin = false,
            IdentityContext = tokenInfo.IdentityId,
        });

        return response;

    }


}