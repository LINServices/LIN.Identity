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

        // Servicios de datos.
        services.AddScoped<Data.Accounts, Data.Accounts>();
        services.AddScoped<Data.DirectoryMembers, Data.DirectoryMembers>();
        services.AddScoped<Data.GroupMembers, Data.GroupMembers>();
        services.AddScoped<Data.Groups, Data.Groups>();
        services.AddScoped<Data.Identities, Data.Identities>();
        services.AddScoped<Data.IdentityRoles, Data.IdentityRoles>();
        services.AddScoped<Data.Organizations, Data.Organizations>();
        services.AddScoped<Data.PassKeys, Data.PassKeys>();
        services.AddScoped<Data.AccountLogs, Data.AccountLogs>();
        services.AddScoped<Data.Policies, Data.Policies>();
        services.AddScoped<Data.PoliciesRequirement, Data.PoliciesRequirement>();
        services.AddScoped<Data.OtpService, Data.OtpService>();
        services.AddScoped<Data.Mails, Data.Mails>();
        services.AddScoped<Data.ApplicationRestrictions, Data.ApplicationRestrictions>();

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