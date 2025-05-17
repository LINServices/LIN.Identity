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

        var pipelineSteps = new List<Type>
            {
                typeof(IIdentityValidationService),
                typeof(IOrganizationValidationService),
                typeof(IApplicationValidationService)
            };

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
    /// Generar token de autenticación.
    /// </summary>
    public string GenerateToken()
    {
        if (Request.Account is null || Request.ApplicationModel is null)
            return string.Empty;

        return JwtService.Generate(Request.Account!, Request.ApplicationModel.Id);
    }
}