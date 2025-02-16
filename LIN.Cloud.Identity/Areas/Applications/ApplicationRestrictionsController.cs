namespace LIN.Cloud.Identity.Areas.Applications;

[IdentityToken]
[Route("applications/restrictions")]
public class ApplicationRestrictionsController(Data.ApplicationRestrictions applicationRestrictions) : AuthenticationBaseController
{

    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromBody] ApplicationRestrictionModel app)
    {

        // Si el modelo es nulo.
        if (app is null)
            return new(Responses.InvalidParam)
            {
                Errors = [new() { Tittle = "Modelo invalido", Description = "El modelo json es invalido." }]
            };

        app.Application = new()
        {
            Id = app.ApplicationId
        };

        var create = await applicationRestrictions.Create(app);

        return create;
    }

}