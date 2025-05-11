using LIN.Cloud.Identity.Persistence.Repositories;

namespace LIN.Cloud.Identity.Areas.Applications;

[IdentityToken]
[Route("applications")]
public class ApplicationsController(IApplicationRepository application) : AuthenticationBaseController
{

    /// <summary>
    /// Crear nueva aplicación.
    /// </summary>
    /// <param name="app">App.</param>
    [HttpPost]
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
        app.Policies = new();
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

}