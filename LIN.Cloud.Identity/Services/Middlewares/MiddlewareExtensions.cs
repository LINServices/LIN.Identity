namespace LIN.Cloud.Identity.Services.Middlewares;


public static class MiddlewareExtensions
{

    /// <summary>
    /// Agregar servicio.
    /// </summary>
    public static IServiceCollection AddIP(this IServiceCollection builder) => builder.AddSingleton<IPMiddleware>();


    /// <summary>
    /// Activar el servicio.
    /// </summary>
    public static IApplicationBuilder UseIP(this IApplicationBuilder builder) => builder.UseMiddleware<IPMiddleware>();


    /// <summary>
    /// Agregar servicio.
    /// </summary>
    public static IServiceCollection AddQuota(this IServiceCollection builder) => builder.AddSingleton<QuotaMiddleware>();


    /// <summary>
    /// Activar el servicio.
    /// </summary>
    public static IApplicationBuilder UseQuota(this IApplicationBuilder builder, int max)
    {
        if (max <= 0)
            max = 1;

        QuotaMiddleware.MaxQuote = max;
        return builder.UseMiddleware<QuotaMiddleware>();
    }


}