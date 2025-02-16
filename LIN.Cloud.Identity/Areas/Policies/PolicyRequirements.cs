namespace LIN.Cloud.Identity.Areas.Policies;

[IdentityToken]
[Route("[controller]")]
public class PolicyRequirementsController(Data.PoliciesRequirement policiesData, IamPolicy iamPolicy) : AuthenticationBaseController
{

    /// <summary>
    /// Crear nueva política.
    /// </summary>
    /// <param name="modelo">Modelo de la identidad.</param>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromBody] PolicyRequirementModel modelo)
    {

        // Validar roles.
        var roles = await iamPolicy.Validate(UserInformation.IdentityId, modelo.PolicyId.ToString());

        if (roles != IamLevels.Privileged)
            return new(Responses.Unauthorized) { Message = $"No tienes permisos" };

        // Forma
        var response = await policiesData.Create(modelo);
        return response;
    }

}