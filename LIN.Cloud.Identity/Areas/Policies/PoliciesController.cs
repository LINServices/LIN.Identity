using LIN.Cloud.Identity.Persistence.Repositories.Mongo;
using LIN.Types.Cloud.Identity.Models.Policies;

namespace LIN.Cloud.Identity.Areas.Policies;

[IdentityToken]
[Route("[controller]")]
public class PoliciesController(PolicyRepository policiesData, IIamService iam) : AuthenticationBaseController
{

    /// <summary>
    /// Crear nueva política.
    /// </summary>
    /// <param name="modelo">Modelo de la identidad.</param>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromBody] AccessPolicyModel modelo, [FromHeader] int organization)
    {
        // Validar nivel de acceso y roles sobre la organización.
        var validate = await iam.Validate(UserInformation.IdentityId, organization);

        if (!validate.ValidateAlterPolicies())
            return new(Responses.Unauthorized)
            {
                Message = $"No tienes permisos para crear políticas a titulo de la organización #{organization}."
            };

        modelo.OrganizationId = organization;
        // Crear la política.
        var response = await policiesData.Create(modelo);
        return response;
    }

    /// <summary>
    /// Obtener las políticas asociadas a una organización.
    /// </summary>
    [HttpGet("all")]
    public async Task<HttpReadAllResponse<AccessPolicyModel>> ReadAll([FromHeader] int organization)
    {
        // Validar nivel de acceso y roles sobre la organización.
        var validate = await iam.Validate(UserInformation.IdentityId, organization);

        if (!validate.ValidateReadPolicies())
            return new(Responses.Unauthorized)
            {
                Message = $"No tienes permisos para leer políticas a titulo de la organización #{organization}."
            };

        // Validar nivel de acceso (esto podría requerir una validación IAM más específica para la identidad).
        var response = await policiesData.ReadAllByOrg(organization,query: null);
        return response;
    }


    /// <summary>
    /// Obtener las políticas asociadas a una organización.
    /// </summary>
    [HttpGet]
    public async Task<HttpReadOneResponse<AccessPolicyModel>> Read([FromQuery] string policyId)
    {
        // Validar nivel de acceso (esto podría requerir una validación IAM más específica para la identidad).
        var response = await policiesData.ReadOne(policyId);
        return response;
    }

    /// <summary>
    /// Obtener las políticas asociadas a una organización.
    /// </summary>
    [HttpGet("search")]
    public async Task<HttpReadAllResponse<AccessPolicyModel>> Search([FromHeader] int organization, [FromQuery] string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
            query = null;

        // Validar nivel de acceso (esto podría requerir una validación IAM más específica para la identidad).
        var response = await policiesData.ReadAllByOrg(organization, query);
        return response;
    }

    /// <summary>
    /// Eliminar una política.
    /// </summary>
    /// <param name="id">Id de la política (ObjectId string).</param>
    [HttpDelete]
    public async Task<HttpResponseBase> Delete([FromHeader] string id)
    {
        var response = await policiesData.Delete(id);
        return response;
    }
}