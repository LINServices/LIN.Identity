using LIN.Cloud.Identity.Services.Services.Authentication;
using LIN.Cloud.Identity.Services.Services.Iam;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LIN.Cloud.Identity.Services.Extensions;

public static class ServiceExtensions
{

    /// <summary>
    /// Agregar servicios de persistence.
    /// </summary>
    /// <param name="services">Services.</param>
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Servicios de datos.
        services.AddScoped<IApplicationValidationService, ApplicationValidationService>();
        services.AddScoped<IOrganizationValidationService, OrganizationValidationService>();
        services.AddScoped<IIdentityValidationService, IdentityValidationService>();
        services.AddScoped<IIdentityService, Services.IdentityService>();

        services.AddScoped<IAuthenticationAccountService, AccountAuthenticationService>();
        services.AddScoped<IPolicyOrchestrator, PolicyOrchestrator>();
        services.AddScoped<IIamService, IamService>();

        JwtService.Open(configuration);
        JwtApplicationsService.Open(configuration);

        return services;
    }

}