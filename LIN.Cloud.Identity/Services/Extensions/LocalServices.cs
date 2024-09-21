using LIN.Cloud.Identity.Services.Auth.Interfaces;
using LIN.Cloud.Identity.Services.Utils;

namespace LIN.Cloud.Identity.Services.Extensions;


public static class LocalServices
{

    /// <summary>
    /// Agregar servicios locales.
    /// </summary>
    /// <param name="services">Services.</param>
    public static IServiceCollection AddLocalServices(this IServiceCollection services)
    {

        services.AddScoped<Data.Accounts, Data.Accounts>();
        services.AddScoped<Data.DirectoryMembers, Data.DirectoryMembers>();
        services.AddScoped<Data.GroupMembers, Data.GroupMembers>();
        services.AddScoped<Data.Groups, Data.Groups>();
        services.AddScoped<Data.Identities, Data.Identities>();
        services.AddScoped<Data.IdentityRoles, Data.IdentityRoles>();
        services.AddScoped<Data.Organizations, Data.Organizations>();
        services.AddScoped<Data.PassKeys, Data.PassKeys>();

        // Iam.
        services.AddScoped<RolesIam, RolesIam>();

        // Allow.
        services.AddScoped<IAllowService, AllowService>();
        services.AddScoped<IIdentityService, Utils.IdentityService>();

        return services;

    }

}