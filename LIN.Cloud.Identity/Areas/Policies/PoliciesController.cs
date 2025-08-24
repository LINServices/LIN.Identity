namespace LIN.Cloud.Identity.Areas.Policies;

[IdentityToken]
[Route("[controller]")]
public class PoliciesController(IPolicyRepository policiesData, IIamService iam) : AuthenticationBaseController
{

    /// <summary>
    /// Crear nueva política.
    /// </summary>
    /// <param name="modelo">Modelo de la identidad.</param>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromBody] PolicyModel modelo, [FromHeader] int organization)
    {
        // Validar nivel de acceso y roles sobre la organización.
        var validate = await iam.Validate(UserInformation.IdentityId, organization);

        if (!validate.ValidateAlterPolicies())
            return new(Responses.Unauthorized)
            {
                Message = $"No tienes permisos para crear políticas a titulo de la organización #{organization}."
            };

        // Limpiar modelo.
        modelo.Owner = new() { Id = organization };
        modelo.CreatedBy = new() { Id = UserInformation.IdentityId };
        modelo.CreatedAt = DateTime.UtcNow;
        modelo.Id = 0;
        modelo.Name = modelo.Name.Trim();

        // Crear la política.
        var response = await policiesData.Create(modelo);
        return response;
    }


    /// <summary>
    /// Obtener una política.
    /// </summary>
    /// <param name="policyId">Id.</param>
    [HttpGet]
    public async Task<HttpReadOneResponse<PolicyModel>> Read([FromHeader] int policyId)
    {
        // Validar nivel de acceso y roles sobre la organización.
        var validate = await iam.IamPolicy(UserInformation.IdentityId, policyId);

        if (validate != IamLevels.Privileged)
            return new(Responses.Unauthorized)
            {
                Message = $"No tienes permisos para obtener esta política."
            };

        // Crear la política.
        var response = await policiesData.Read(policyId, true);
        return response;
    }


    /// <summary>
    /// Buscar políticas por nombre.
    /// </summary>
    [HttpGet("search")]
    public async Task<HttpReadAllResponse<PolicyModel>> Search([FromQuery] string query, [FromHeader] int organization)
    {

        // Validar nivel de acceso y roles sobre la organización.
        var validate = await iam.Validate(UserInformation.IdentityId, organization);

        if (!validate.ValidateReadPolicies())
            return new(Responses.Unauthorized)
            {
                Message = $"No tienes permisos para obtener políticas a titulo de la organización #{organization}."
            };

        // Crear la política.
        var response = await policiesData.ReadAll(organization, query);
        return response;
    }


    /// <summary>
    /// Obtener las políticas asociadas a una organización.
    /// </summary>
    /// <param name="organization">Id de la organización.</param>
    [HttpGet("all")]
    public async Task<HttpReadAllResponse<PolicyModel>> ReadAll([FromHeader] int organization)
    {

        // Validar nivel de acceso y roles sobre la organización.
        var validate = await iam.Validate(UserInformation.IdentityId, organization);

        if (!validate.ValidateReadPolicies())
            return new(Responses.Unauthorized)
            {
                Message = $"No tienes permisos para obtener políticas a titulo de la organización #{organization}."
            };

        // Crear la política.
        var response = await policiesData.ReadAll(organization, false);
        return response;
    }

}