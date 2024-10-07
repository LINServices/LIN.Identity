namespace LIN.Cloud.Identity.Services.Middlewares;

public class QuotaMiddleware : IMiddleware
{

    /// <summary>
    /// Solicites máximas permitidas al momento.
    /// </summary>
    public static int MaxQuote { get; set; }


    /// <summary>
    /// Solicitudes que se están procesando actualmente.
    /// </summary>
    private static volatile int ActualQuote = 0;


    /// <summary>
    /// Invocación del Middleware.
    /// </summary>
    /// <param name="context">Contexto HTTP.</param>
    /// <param name="next">Pipeline</param>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {

        // Comprobar el limite de cuota.
        if (ActualQuote >= MaxQuote)
        {
            context.Response.StatusCode = 503;
            await context.Response.WriteAsJsonAsync(new ResponseBase
            {
                Message = $"Actualmente estamos al limite de solicitudes.",
                Response = Responses.UnavailableService
            });
            return;
        }

        // Incrementar.
        ActualQuote++;

        // Ejecución del flujo (Pipeline).
        await next(context);

        // Decrementar.
        ActualQuote--;

    }


}