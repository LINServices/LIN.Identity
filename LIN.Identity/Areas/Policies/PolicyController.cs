namespace LIN.Identity.Areas.Organizations;


[Route("policies")]
public class PolicyController : ControllerBase
{


    /// <summary>
    /// Valida el acceso a un permiso de una identidad.
    /// </summary>
    /// <param name="identity">ID de la identidad</param>
    /// <param name="policy">ID de la política de permisos</param>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromBody] PolicyModel policy, [FromHeader] string token)
    {

        // Validación del token
        var (isValid, _, _, _, _, _, _) = Jwt.Validate(token);;

        // Token es invalido
        if (!isValid)
        {
            return new CreateResponse
            {
                Message = "Token invalido.",
                Response = Responses.Unauthorized
            };
        }


    }



    /// <summary>
    /// Valida el acceso a un permiso de una identidad.
    /// </summary>
    /// <param name="identity">ID de la identidad</param>
    /// <param name="policy">ID de la política de permisos</param>
    [HttpGet("access")]
    public async Task<HttpReadOneResponse<bool>> ReadAll([FromQuery] int identity, [FromQuery] int policy)
    {

        // Validar parámetros.
        if (identity <= 0 || policy <=0)
            return new()
            {
                Message = "Parámetros inválidos.",
                Response = Responses.Unauthorized
            };

        // Respuesta.
        return await Data.Policies.ValidatePermission(identity, policy);

    }



}