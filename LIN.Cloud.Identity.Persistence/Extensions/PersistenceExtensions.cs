using LIN.Cloud.Identity.Persistence.Contexts;
using LIN.Cloud.Identity.Persistence.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LIN.Cloud.Identity.Persistence.Extensions;

public static class PersistenceExtensions
{

    /// <summary>
    /// Agregar servicios de persistence.
    /// </summary>
    /// <param name="services">Services.</param>
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfigurationManager configuration)
    {
        string? connectionName = "cloud";
#if LOCAL
        connectionName = "local";
#elif DEBUG_DEV
        connectionName = "cloud-dev";
#elif RELEASE_DEV
        connectionName = "cloud-dev";
#endif

        services.AddDbContextPool<DataContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString(connectionName));
        });

        services.AddScoped<AccountFindable, AccountFindable>();
        services.AddScoped<IdentityFindable, IdentityFindable>();

        return services;
    }


    /// <summary>
    /// Habilitar el servicio de base de datos.
    /// </summary>
    public static IApplicationBuilder UseDataBase(this IApplicationBuilder app)
    {
      
        var scope = app.ApplicationServices.CreateScope();
        var logger = scope.ServiceProvider.GetService<ILogger<DataContext>>();
        try
        {
          
            var context = scope.ServiceProvider.GetService<DataContext>();
            bool? created = context?.Database.EnsureCreated();

            // Data seed.
            context?.Seed();
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error al definir la base de datos.");
        }
        return app;
    }

}