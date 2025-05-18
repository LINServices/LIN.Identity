using LIN.Cloud.Identity.Persistence.Queries;
using LIN.Cloud.Identity.Persistence.Repositories;
using LIN.Cloud.Identity.Persistence.Repositories.EntityFramework;
using Microsoft.AspNetCore.Builder;
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
        string? connectionName = "cloud-v4";
#if LOCAL
        connectionName = "cloud-v4";
#elif DEBUG_DEV
        connectionName = "cloud-v4";
#elif RELEASE_DEV
        connectionName = "cloud-v4";
#endif

        services.AddDbContextPool<DataContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString(connectionName));
        });

        services.AddScoped<AccountFindable, AccountFindable>();
        services.AddScoped<IdentityFindable, IdentityFindable>();

        // Servicios de datos.
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IAccountLogRepository, AccountLogRepository>();
        services.AddScoped<IApplicationRepository, ApplicationRepository>();
        services.AddScoped<IGroupMemberRepository, GroupMemberRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IIdentityRepository, IdentityRepository>();
        services.AddScoped<IIdentityRolesRepository, IdentityRolesRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>();
        services.AddScoped<IOtpRepository, OtpRepository>();
        services.AddScoped<IPolicyRepository, PolicyRepository>();
        services.AddScoped<IPolicyMemberRepository, PolicyMemberRepository>();

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