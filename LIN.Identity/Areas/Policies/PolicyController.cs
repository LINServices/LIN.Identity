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

        // Token.
        var tokenInfo = Jwt.Validate(token);

        // Si el token no es valido.
        if (!tokenInfo.IsAuthenticated)
            return new()
            {
                Response = Responses.Unauthorized,
                Message = "Token invalido."
            };

        // Acceso IAM.
        // var iam = await Services.Iam.Directories.ValidateAccess(identity, policy.DirectoryId);

        // SI no tiene permisos de modificación.
        //if (iam.Model != IamLevels.Privileged)
        //    return new CreateResponse
        //    {
        //        Message = "No tienes permiso para modificar este directorio.",
        //        Response = Responses.Unauthorized
        //    };


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



    /// <summary>
    /// Obtiene las políticas asociadas a un directorio.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="directory">Id del directorio.</param>
    [HttpGet("read/all")]
    public async Task<HttpReadAllResponse<PolicyModel>> ReadAll([FromHeader] string token, [FromQuery] int directory)
    {

        // Validar parámetros.
        if (directory <= 0)
            return new()
            {
                Message = "Parámetros inválidos.",
                Response = Responses.InvalidParam
            };

        // Validar JSON.
        var (isValid, _, _, _, _, identity) = Jwt.Validate(token);

        // Token es invalido.
        if (!isValid)
            return new()
            {
                Message = "Token invalido.",
                Response = Responses.Unauthorized
            };

        // Acceso IAM.
        var (_, _, roles) = await Queries.Directories.Get(identity);

        // Si no hay acceso.
        if (Roles.ViewPolicy(roles))
            return new()
            {
                Response = Responses.Unauthorized,
                Message = "No tienes permisos para visualizar las políticas de este recurso."
            };

        // Respuesta.
        return await Data.Policies.ReadAll(directory);

    }



}