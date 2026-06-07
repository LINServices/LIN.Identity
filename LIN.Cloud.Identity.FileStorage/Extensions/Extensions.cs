using LIN.Cloud.Identity.FileStorage.Abstractions;
using LIN.Cloud.Identity.FileStorage.Implementations;
using LIN.Cloud.Identity.FileStorage.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LIN.Cloud.Identity.FileStorage.Extensions;

public static class Extensions
{
    /// <summary>
    /// Agregar servicios de almacenamiento de archivos.
    /// </summary>
    /// <param name="services">Servicios.</param>
    /// <param name="configuration">Configuración.</param>
    public static IServiceCollection AddFileStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FileStorageOptions>(configuration.GetSection("FileStorage"));
        services.AddSingleton<IFileStorageService, MinioFileStorageService>();
        return services;
    }
}