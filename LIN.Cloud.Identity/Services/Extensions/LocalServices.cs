namespace LIN.Cloud.Identity.Services.Extensions;

public static class LocalServices
{

    /// <summary>
    /// Agregar servicios locales.
    /// </summary>
    /// <param name="services">Services.</param>
    public static IServiceCollection AddLocalServices(this IServiceCollection services)
    {
        // Externos
        services.AddSingleton<EmailSender, EmailSender>();
        return services;
    }

}