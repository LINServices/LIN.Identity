namespace LIN.Identity.Areas.Organizations;


[Route("policies")]
public class PolicyController : ControllerBase
{


    /// <summary>
    /// Crear política.
    /// </summary>
    /// <param name="policy">Modelo.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromBody] PolicyModel policy, [FromHeader] string token)
    {

        // Validar parámetros.
        if (policy == null || string.IsNullOrEmpty(token) || policy.DirectoryId <= 0)
            return new CreateResponse
            {
                Message = "Parámetros inválidos.",
                Response = Responses.InvalidParam
            };

        // Validación del token
        var (isValid, _, _, _, _, identity) = Jwt.Validate(token); ;

        // Token es invalido
        if (!isValid)
            return new CreateResponse
            {
                Message = "Token invalido.",
                Response = Responses.Unauthorized
            };


        // Acceso IAM.
        var iam = await Services.Iam.Directories.ValidateAccess(identity, policy.DirectoryId);

        // SI no tiene permisos de modificación.
        if (iam.Model != IamLevels.Privileged)
            return new CreateResponse
            {
                Message = "No tienes permiso para modificar este directorio.",
                Response = Responses.Unauthorized
            };


        return await Data.Policies.Create(new()
        {
            Creation = DateTime.Now,
            Directory = new()
            {
                ID = policy.DirectoryId,
            },
            Id = 0,
            Type = policy.Type,
            ValueJson = policy.ValueJson,
        });

    }



    /// <summary>
    /// Valida el acceso a un permiso de una identidad.
    /// </summary>
    /// <param name="identity">ID de la identidad</param>
    /// <param name="policy">ID de la política de permisos</param>
    [HttpGet("access")]
    public async Task<HttpReadOneResponse<bool>> ValidatePermissions([FromQuery] int identity, [FromQuery] int policy)
    {

        // Validar parámetros.
        if (identity <= 0 || policy <= 0)
            return new()
            {
                Message = "Parámetros inválidos.",
                Response = Responses.Unauthorized
            };

        // Respuesta.
        return await Data.Policies.ValidatePermission(identity, policy);

    }



}