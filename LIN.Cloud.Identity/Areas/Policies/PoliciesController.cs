namespace LIN.Cloud.Identity.Areas.Policies;

[Route("[controller]")]
public class PoliciesController(Data.Policies policiesData, Data.Groups groups, RolesIam iam) : AuthenticationBaseController
{

    /// <summary>
    /// Crear nueva política.
    /// </summary>
    /// <param name="modelo">Modelo de la identidad.</param>
    [HttpPost]
    [IdentityToken]
    public async Task<HttpCreateResponse> Create([FromBody] PolicyModel modelo)
    {

        // Si ya tiene una identidad.
        if (modelo.OwnerIdentityId > 0)
        {
            // Obtener detalles.
            var owner = await groups.GetOwnerByIdentity(modelo.OwnerIdentityId);

            if (owner.Response != Responses.Success)
                return new(Responses.NotRows) { Message = $"No se encontró la organización del grupo con identidad {modelo.OwnerIdentityId}" };

            // Validar roles.
            var roles = await iam.RolesOn(AuthenticationInformation.IdentityId, owner.Model);

            bool hasPermission = ValidateRoles.ValidateAlterMembers(roles);

            if (!hasPermission)
                return new(Responses.Unauthorized) { Message = $"No tienes permisos para crear políticas a titulo de la organización #{owner.Model}." };

        }
        else
        {
            // Establecer propietario al usuario que realiza la solicitud.
            modelo.OwnerIdentityId = AuthenticationInformation.IdentityId;
        }

        // Formatear.
        modelo.OwnerIdentity = new()
        {
            Id = modelo.OwnerIdentityId
        };

        modelo.ApplyFor = [];

        var response = await policiesData.Create(modelo);
        return response;
    }


    /// <summary>
    /// Validar si tiene autorización.
    /// </summary>
    /// <param name="policy">Id de la política.</param>
    [HttpGet("isAllow")]
    [IdentityToken]
    public async Task<ResponseBase> IsAllow([FromQuery] string policy)
    {
        var response = await policiesData.HasFor(AuthenticationInformation.IdentityId, policy);
        return response;
    }


    /// <summary>
    /// Validar si tiene acceso a una política.
    /// </summary>
    /// <param name="policy">Id de la política.</param>
    /// <param name="identity">Id de la identidad.</param>
    [HttpGet("isAllow/identity")]
    [IdentityToken]
    public async Task<ResponseBase> IsAllow([FromQuery] string policy, [FromHeader] int identity)
    {
        var response = await policiesData.HasFor(identity, policy);
        return response;
    }

}