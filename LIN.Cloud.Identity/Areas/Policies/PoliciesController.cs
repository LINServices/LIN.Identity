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

}