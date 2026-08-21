namespace LIN.Cloud.Identity.Areas.Accounts;

[Route("[controller]")]
public class AccountController(IAccountRepository accountData, IApplicationRepository applications, IFileStorageService fileStorage) : AuthenticationBaseController
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

        if (errors.Count > 0)
            return new(Responses.InvalidParam)
            {
                Message = "Ocurrió un error al crear la cuenta",
                Errors = errors
            };

        // Organización del modelo.
        modelo.AccountType = AccountTypes.Personal;
        modelo = Services.Formats.Account.Process(modelo);

        var response = await accountData.Create(modelo, 0);

        if (response.Response != Responses.Success)
            return new(response.Response)
            {
                Message = "Hubo un error al crear la cuenta."
            };

        var application = await applications.Read(app);

        string token = string.Empty;

        // Si la aplicación es valida, generamos un token.
        if (application.Response == Responses.Success)
            token = JwtService.Generate(response.Model, application.Model.Id);

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
    [HttpGet("{id:int}")]
    [IdentityToken]
    public async Task<HttpReadOneResponse<AccountModel>> Read([FromRoute] int id)
    {
        if (id <= 0)
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son inválidos."
            };

        var response = await accountData.Read(id, new()
        {
            AccountContext = UserInformation.AccountId,
            FindOn = FindOn.StableAccounts,
            IdentityContext = UserInformation.IdentityId,
            IsAdmin = false
        });

        if (response.Response != Responses.Success)
            return new ReadOneResponse<AccountModel>()
            {
                Response = response.Response
            };

        await ResolveProfileAsync(response.Model);

        return response;
    }

    /// <summary>
    /// Obtener una cuenta.
    /// </summary>
    /// <param name="user">Unique de la identidad de la cuenta.</param>
    /// <returns>Retorna el modelo de la cuenta.</returns>
    [HttpGet("user/{user}")]
    [IdentityToken]
    public async Task<HttpReadOneResponse<AccountModel>> Read([FromRoute] string user)
    {
        if (string.IsNullOrWhiteSpace(user))
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son inválidos."
            };

        var response = await accountData.Read(user, new()
        {
            AccountContext = UserInformation.AccountId,
            FindOn = FindOn.StableAccounts,
            IdentityContext = UserInformation.IdentityId,
            IsAdmin = false
        });

        if (response.Response != Responses.Success)
            return new ReadOneResponse<AccountModel>()
            {
                Response = response.Response,
                Model = new()
            };

        await ResolveProfileAsync(response.Model);

        return response;
    }

    /// <summary>
    /// Obtener una cuenta.
    /// </summary>
    /// <param name="id">Id de la identidad.</param>
    /// <returns>Retorna el modelo de la cuenta.</returns>
    [HttpGet("identity/{id:int}")]
    [IdentityToken]
    public async Task<HttpReadOneResponse<AccountModel>> ReadByIdentity([FromRoute] int id)
    {
        if (id <= 0)
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son inválidos."
            };

        var response = await accountData.ReadByIdentity(id, new()
        {
            AccountContext = UserInformation.AccountId,
            FindOn = FindOn.StableAccounts,
            IdentityContext = UserInformation.IdentityId,
            IsAdmin = false
        });

        if (response.Response != Responses.Success)
            return new ReadOneResponse<AccountModel>()
            {
                Response = response.Response
            };

        await ResolveProfileAsync(response.Model);

        return response;
    }

    /// <summary>
    /// Obtener una lista de cuentas según las id de las identidades.
    /// </summary>
    /// <param name="ids">Id de las identidades</param>
    /// <returns>Retorna la lista de cuentas.</returns>
    [HttpPost("identity")]
    [IdentityToken]
    public async Task<HttpReadAllResponse<AccountModel>> ReadByIdentity([FromBody] List<int> ids)
    {
        var response = await accountData.FindAllByIdentities(ids, new()
        {
            AccountContext = UserInformation.AccountId,
            FindOn = FindOn.StableAccounts,
            IsAdmin = false,
            IdentityContext = UserInformation.IdentityId,
        });

        foreach (var a in response.Models)
        {
            await ResolveProfileAsync(a);
        }

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
        if (pattern.Trim().Length <= 0 || string.IsNullOrWhiteSpace(pattern))
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son inválidos."
            };

        var response = await accountData.Search(pattern, new()
        {
            AccountContext = UserInformation.AccountId,
            FindOn = FindOn.StableAccounts,
            IsAdmin = false,
            IdentityContext = UserInformation.IdentityId,
        });

        foreach (var a in response.Models)
        {
            await ResolveProfileAsync(a);
        }

        return response;
    }

    /// <summary>
    /// Obtener la lista de cuentas.
    /// </summary>
    /// <param name="ids">Lista de ids de las cuentas.</param>
    /// <returns>Retorna una lista de las cuentas encontradas.</returns>
    [HttpPost("batch")]
    [IdentityToken]
    public async Task<HttpReadAllResponse<AccountModel>> ReadAll([FromBody] List<int> ids)
    {
        var response = await accountData.FindAll(ids, new()
        {
            AccountContext = UserInformation.AccountId,
            FindOn = FindOn.StableAccounts,
            IsAdmin = false,
            IdentityContext = UserInformation.IdentityId,
        });

        return response;
    }

    /// <summary>
    /// Actualizar la foto de perfil de una cuenta.
    /// </summary>
    /// <param name="file">Archivo de imagen.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Retorna el resultado de la operación.</returns>
    [HttpPatch("profile")]
    [IdentityToken]
    public async Task<HttpResponseBase> UpdateProfile(IFormFile file, CancellationToken cancellationToken)
    {
        // Validar el archivo.
        if (file is null || file.Length == 0)
            return new(Responses.InvalidParam)
            {
                Message = "Uno o varios parámetros son inválidos."
            };

        // Generar nombre único para el archivo.
        var extension = Path.GetExtension(file.FileName);
        var objectName = $"accounts/{UserInformation.AccountId}/{Guid.NewGuid()}{extension}";

        using var stream = file.OpenReadStream();
        var uploadResult = await fileStorage.UploadFileAsync("profiles", objectName, stream, file.Length, file.ContentType, cancellationToken);

        if (!uploadResult.IsSuccess)
            return new(Responses.UnavailableService)
            {
                Message = "Hubo un error al subir la foto de perfil."
            };

        var response = await accountData.UpdateProfile(UserInformation.AccountId, uploadResult.Url!);

        if (response.Response != Responses.Success)
            return new(response.Response)
            {
                Message = "Hubo un error al actualizar la foto de perfil."
            };

        return new(Responses.Success)
        {
            Message = "Foto de perfil actualizada satisfactoriamente."
        };
    }

    /// <summary>
    /// Reemplaza la URL interna del perfil por una URL pública temporal.
    /// </summary>
    /// <param name="account">Modelo de la cuenta.</param>
    private async Task ResolveProfileAsync(AccountModel account)
    {
        if (string.IsNullOrWhiteSpace(account.Profile))
            return;

        var result = await fileStorage.GetTemporaryUrlAsync(account.Profile, TimeSpan.FromDays(1));
        if (result.IsSuccess)
            account.Profile = result.Url!;
    }

}