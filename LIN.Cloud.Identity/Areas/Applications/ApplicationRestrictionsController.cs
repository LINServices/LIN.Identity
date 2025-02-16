namespace LIN.Cloud.Identity.Areas.Applications;

[IdentityToken]
[Route("applications/restrictions")]
public class ApplicationRestrictionsController(Data.ApplicationRestrictions applicationRestrictions) : AuthenticationBaseController
{

    /// <summary>
    /// Crear restricción.
    /// </summary>
    /// <param name="app">Modelo.</param>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromBody] ApplicationRestrictionModel app)
    {

        // Si el modelo es nulo.
        if (app is null)
            return new(Responses.InvalidParam)
            {
                Errors = [new() { Tittle = "Modelo invalido", Description = "El modelo json es invalido." }]
            };

        // Modelo.
        app.Application = new()
        {
            Id = app.ApplicationId
        };

        // Crear restricción.
        var create = await applicationRestrictions.Create(app);
        return create;
    }

}