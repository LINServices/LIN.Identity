using LIN.Types.Cloud.Identity.Models.Policies;

namespace LIN.Cloud.Identity.Areas.Policies;

[IdentityToken]
[Route("[controller]")]
public class PoliciesIdentityController(IPolicyMemberRepository policiesData, IIamService iam) : AuthenticationBaseController
{

    /// <summary>
    /// Asociar política.
    /// </summary>
    /// <param name="modelo">Modelo de la identidad.</param>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromQuery] int identity, [FromQuery] string policy)
    {
        // Validar nivel de acceso y roles sobre la organización.
        var validate = await iam.IamIdentity(UserInformation.IdentityId, identity);

        if (!validate.ValidateAlterPolicies())
            return new(Responses.Unauthorized)
            {
                Message = $"No tienes permisos modificar la identidad y agregarla a una política."
            };

        // Crear la política.
        var response = await policiesData.Create(policy, identity);
        return response;
    }

    /// <summary>
    /// Obtener las políticas asociadas a una identidad.
    /// </summary>
    [HttpGet("all")]
    public async Task<HttpReadAllResponse<AccessPolicyModel>> ReadAll([FromHeader] int identity)
    {
        // Validar nivel de acceso y roles sobre la organización.
        var validate = await iam.IamIdentity(UserInformation.IdentityId, identity);

        if (!validate.ValidateReadPolicies())
            return new(Responses.Unauthorized)
            {
                Message = $"No tienes permisos para obtener políticas a titulo de la organización."
            };

        // Crear la política.
        var response = await policiesData.ReadAll(identity);
        return response;
    }

    /// <summary>
    /// Eliminar una identidad de una politica
    /// </summary>
    [HttpDelete]
    public async Task<HttpResponseBase> Delete([FromHeader] int identity, [FromQuery] string policy)
    {
        // Validar nivel de acceso y roles sobre la organización.
        var validate = await iam.IamIdentity(UserInformation.IdentityId, identity);

        if (!validate.ValidateAlterPolicies())
            return new(Responses.Unauthorized)
            {
                Message = $"No tienes permisos para obtener políticas a titulo de la organización."
            };

        // Crear la política.
        var response = await policiesData.Remove(policy, identity);
        return response;
    }
}