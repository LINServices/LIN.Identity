﻿using LIN.Cloud.Identity.Persistence.Contexts;
using LIN.Cloud.Identity.Persistence.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LIN.Cloud.Identity.Persistence.Extensions;

public static class PersistenceExtensions
{


    /// <summary>
    /// Agregar servicios de persistence.
    /// </summary>
    /// <param name="services">Services.</param>
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfigurationManager configuration)
    {


        services.AddDbContextPool<DataContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("cloud"));
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
        try
        {

            // Obtener servicio.
            var context = app.ApplicationServices.GetService<DataContext>();

            context?.Database.EnsureCreated() ;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        return app;
    }



}