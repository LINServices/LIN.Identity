namespace LIN.Identity.Services.Filters;


public class IPMiddleware : IMiddleware
{

    /// <summary>
    /// Middleware de IP.
    /// </summary>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Obtener la IP.
        var ip = context.Connection.RemoteIpAddress;

        // Validar la IP.
        if (ip == null)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new ResponseBase
            {
                Message = "La IP del cliente no pudo ser resuelta.",
                Response = Responses.Unauthorized
            });
            return;
        }

        // Item de IP.
        context.Items.Add("IP", ip);
        await next(context);
    }

}


public static class IPMiddlewareExtensions
{
    public static IApplicationBuilder UseIP(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<IPMiddleware>();
    }

    public static IServiceCollection AddIP(this IServiceCollection builder)
    {
        return builder.AddSingleton<IPMiddleware>();
    }
}