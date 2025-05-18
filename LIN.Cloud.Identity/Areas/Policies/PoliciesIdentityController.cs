namespace LIN.Cloud.Identity.Areas.Policies;

[IdentityToken]
[Route("[controller]")]
public class PoliciesIdentityController(IPolicyMemberRepository policiesData, IIamService iam) : AuthenticationBaseController
{

    /// <summary>
    /// Crear nueva política.
    /// </summary>
    /// <param name="modelo">Modelo de la identidad.</param>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromBody] IdentityPolicyModel modelo)
    {

        // Validar nivel de acceso y roles sobre la organización.
        var validate = await iam.IamIdentity(UserInformation.IdentityId, modelo.IdentityId);

        if (!validate.ValidateAlterPolicies())
            return new(Responses.Unauthorized)
            {
                Message = $"No tienes permisos modificar la identidad y agregarla a una política."
            };

        // Ajustar modelo.
        modelo.Policy = new() { Id = modelo.PolicyId };
        modelo.Identity = new() { Id = modelo.IdentityId };

        // Crear la política.
        var response = await policiesData.Create(modelo);
        return response;
    }

}