namespace LIN.Cloud.Identity.Services.Extensions;


public static class LocalServices
{



    /// <summary>
    /// Agregar servicios locales.
    /// </summary>
    /// <param name="services">Services.</param>
    public static IServiceCollection AddLocalServices(this IServiceCollection services)
    {

        services.AddSingleton<Data.Accounts, Data.Accounts>();  
        services.AddSingleton<Data.DirectoryMembers, Data.DirectoryMembers>();  
        services.AddSingleton<Data.GroupMembers, Data.GroupMembers>();  
        services.AddSingleton<Data.Groups, Data.Groups>();  
        services.AddSingleton<Data.Identities, Data.Identities>();  
        services.AddSingleton<Data.IdentityRoles, Data.IdentityRoles>();  
        services.AddSingleton<Data.Organizations, Data.Organizations>();  
        services.AddSingleton<Data.PassKeys, Data.PassKeys>();  

        return services;

    }


}