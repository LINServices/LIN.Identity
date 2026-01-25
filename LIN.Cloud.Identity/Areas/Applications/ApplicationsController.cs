namespace LIN.Cloud.Identity.Areas.Applications;

[Route("applications")]
public class ApplicationsController(IApplicationRepository application) : AuthenticationBaseController
{

    /// <summary>
    /// Crear nueva aplicación.
    /// </summary>
    /// <param name="app">App.</param>
    [HttpPost]
    [IdentityToken]
    public async Task<HttpCreateResponse> Create([FromBody] ApplicationModel app)
    {
        // Si el modelo es nulo.
        if (app is null)
            return new(Responses.InvalidParam)
            {
                Errors = [new() { Tittle = "Modelo invalido", Description = "El modelo json es invalido." }]
            };

        // Validar identidad.
        if (app.Identity is null)
            return new(Responses.InvalidParam)
            {
                Errors = [new() { Tittle = "Identidad invalida", Description = "La identidad es invalida." }]
            };

        if (string.IsNullOrWhiteSpace(app.Identity.Unique))
            return new(Responses.InvalidParam)
            {
                Errors = [new() { Tittle = "Unique invalido", Description = "La identidad unica es invalida (No puede ser vacío)." }]
            };

        // Validar otros parámetros.
        if (string.IsNullOrWhiteSpace(app.Name))
            return new(Responses.InvalidParam)
            {
                Errors = [new() { Tittle = "Nombre invalido", Description = "El nombre de la aplicación no puede estar vacío." }]
            };

        // Formatear app.
        app.Key = Guid.NewGuid();
        app.Identity.Type = IdentityType.Service;
        app.Identity.Roles = [];

        app.Owner = new()
        {
            Id = UserInformation.IdentityId
        };

        Services.Formats.Identities.Process(app.Identity);

        var create = await application.Create(app);

        return create;
    }


    /// <summary>
    /// Solicitar token de acceso a app.
    /// </summary>
    [HttpGet("token")]
    public async Task<HttpResponseBase> RequestToken([FromHeader] string key)
    {
        // Validar key.
        var app = await application.Read(key);

        if (app.Response != Responses.Success)
            return new(Responses.InvalidParam);

        // Generar token de acceso.
        var token = JwtApplicationsService.Generate(app.Model.Id);

        return new ResponseBase
        {
            Response = Responses.Success,
            Token = token
        };
    }


    /// <summary>
    /// Obtener la información básica de la aplicación.
    /// </summary>
    [HttpGet("information")]
    public async Task<HttpReadOneResponse<ApplicationModel>> RequestInformation([FromHeader] string token)
    {

        int id = JwtApplicationsService.Validate(token);

        if (id <= 0)
            return new(Responses.InvalidParam);

        // Validar key.
        var app = await application.Read(id);
        return app;
    }

}