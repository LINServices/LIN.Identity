using LIN.Types.Cloud.Identity.Models.Identities;
using Microsoft.Extensions.DependencyInjection;

namespace LIN.Cloud.Identity.Services.Services;

internal class AccountAuthenticationService(IServiceProvider provider) : IAuthenticationAccountService
{

    /// <summary>
    /// Solicitud de autenticación.
    /// </summary>
    private AuthenticationRequest Request { get; set; } = null!;

    /// <summary>
    /// Obtener la cuenta de usuario.
    /// </summary>
    public AccountModel? Account => Request?.Account;

    /// <summary>
    /// Autenticar una cuenta de usuario.
    /// </summary>
    public async Task<ResponseBase> Authenticate(AuthenticationRequest request)
    {
        Request = request;

        using var scope = provider.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        // Configurar el pipeline de autenticación.
        List<Type> pipelineSteps = [];

        pipelineSteps.AddRange([
               typeof(IIdentityValidationService)
           ]);

        if (request.Service == Types.Cloud.Identity.Enumerations.IdentityService.Google)
        {
            // Agregar pasos específicos para Google.
            pipelineSteps.Insert(0, typeof(IGoogleValidationService));
        }
        else if (request.Service == Types.Cloud.Identity.Enumerations.IdentityService.Microsoft)
        {
            // Agregar pasos específicos para Microsoft.
            //pipelineSteps.Insert(0, );
        }
        else
        {
            // Autenticación por defecto para LIN.
            pipelineSteps.AddRange([
                typeof(IAccountValidationService),
            ]);
        }

        // Pasos comunes para todos los servicios.
        pipelineSteps.AddRange([
                typeof(IOrganizationValidationService),
                typeof(IApplicationValidationService),
                typeof(IIdentityGetService),
                typeof(ISaveLogService)
            ]);

        foreach (var stepType in pipelineSteps)
        {
            var service = (IAuthenticationService)serviceProvider.GetRequiredService(stepType);
            var result = await service.Authenticate(request);

            if (result.Response != Responses.Success)
                return result;
        }

        return new ResponseBase(Responses.Success);
    }

    /// <summary>
    /// Obtener la lista de identidades.
    /// </summary>
    public List<int> Identities => Request.Identities;

    /// <summary>
    /// Generar token de autenticación.
    /// </summary>
    public string GenerateToken()
    {
        if (Request.Account is null || Request.ApplicationModel is null)
            return string.Empty;

        return JwtService.Generate(Request.Account!, Request.ApplicationModel.Id);
    }
}