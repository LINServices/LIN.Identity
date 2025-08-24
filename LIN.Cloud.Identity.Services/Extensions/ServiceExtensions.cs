using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using LIN.Cloud.Identity.Services.Services.Authentication;
using LIN.Cloud.Identity.Services.Services.Authentication.ThirdParties;
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
        // Inicializar Firebase
        //FirebaseApp.Create(new AppOptions
        //{
        //    Credential = GoogleCredential.FromFile("appsettings.firebase.json")
        //});

        // Servicios de datos.
        services.AddScoped<IApplicationValidationService, ApplicationValidationService>();
        services.AddScoped<IOrganizationValidationService, OrganizationValidationService>();
        services.AddScoped<IAccountValidationService, AccountValidationService>();
        services.AddScoped<IIdentityValidationService, IdentityValidationService>();
        services.AddScoped<IGoogleValidationService, GoogleValidationService>();
        services.AddScoped<IIdentityService, Services.IdentityService>();

        services.AddScoped<IAuthenticationAccountService, AccountAuthenticationService>();
        services.AddScoped<IPolicyOrchestrator, PolicyOrchestrator>();
        services.AddScoped<IIamService, IamService>();

        services.AddSingleton<IDomainService, DomainService>();

        JwtService.Open(configuration);
        JwtApplicationsService.Open(configuration);

        return services;
    }

}