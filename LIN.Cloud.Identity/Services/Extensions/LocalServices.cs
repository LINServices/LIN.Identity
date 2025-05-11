using LIN.Cloud.Identity.Services.Auth.Interfaces;

namespace LIN.Cloud.Identity.Services.Extensions;

public static class LocalServices
{

    /// <summary>
    /// Agregar servicios locales.
    /// </summary>
    /// <param name="services">Services.</param>
    public static IServiceCollection AddLocalServices(this IServiceCollection services)
    {

        // Servicios de datos.
        // Externos
        services.AddSingleton<EmailSender, EmailSender>();

        // Iam.
        services.AddScoped<IamRoles, IamRoles>();
        services.AddScoped<IamPolicy, IamPolicy>();

        // Allow.
        services.AddScoped<IAllowService, AllowService>();
        services.AddScoped<IIdentityService, Utils.IdentityService>();
        services.AddScoped<Utils.PolicyService, Utils.PolicyService>();

        return services;

    }

}