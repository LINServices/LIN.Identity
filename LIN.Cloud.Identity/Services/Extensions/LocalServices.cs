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
        services.AddScoped<IamPolicy, IamPolicy>();

        return services;

    }

}