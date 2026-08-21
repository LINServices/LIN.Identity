using LIN.Types.Models;

namespace LIN.Cloud.Identity.Services.Filters;

public class IdentityTokenAttribute : ActionFilterAttribute
{

    /// <summary>
    /// Filtro del token.
    /// </summary>
    /// <param name="context">Contexto HTTP.</param>
    /// <param name="next">Siguiente.</param>
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;

        bool can = httpContext.Request.Headers.TryGetValue("token", out Microsoft.Extensions.Primitives.StringValues value);

        var tokenInfo = JwtService.Validate(value.ToString());

        if (!can || !tokenInfo.IsAuthenticated)
        {
            httpContext.Response.StatusCode = 401;
            await httpContext.Response.WriteAsJsonAsync(new ResponseBase()
            {
                Message = "Token invalido.",
                Errors = [
                    new ErrorModel()
                    {
                        Tittle = "Token invalido",
                        Description = "El token proporcionado en el header <token> es invalido."
                    }
                ],
                Response = Responses.Unauthorized
            });
            return;
        }

        context.HttpContext.Items.Add("authentication", tokenInfo);
        await base.OnActionExecutionAsync(context, next);
    }

}