using LIN.Cloud.Identity.Services.Models;
using LIN.Cloud.Identity.Services.Services.Policies;
using LIN.Types.Cloud.Identity.Models;

namespace LIN.Cloud.Identity.Services.Services;

internal class PolicyOrchestrator(IPolicyRepository policyRepository, IIdentityService identityService) : IPolicyOrchestrator
{

    /// <summary>
    /// Valida las políticas de acceso para una organización.
    /// </summary>
    public async Task<ReadOneResponse<PolicyValidatorContext>> ValidatePoliciesForOrganization(AuthenticationRequest request)
    {
        var context = new PolicyValidatorContext
        {
            AuthenticationRequest = request
        };

        int organization = request.Account!.Identity.OwnerId!.Value;

        // Obtener las identidades.
        var levels = await identityService.GetLevel(request.Account!.Identity.Id, organization);

        foreach (var level in levels.OrderBy(t => t.Key))
        {
            // Obtener las políticas asociadas a la identidad.
            var policies = await policyRepository.ReadAll(level.Value.Select(t => t.Identity), organization, true);

            foreach (var policy in policies.Models)
            {
                ValidateSinglePolicyAsync(policy, context);
            }
        }

        if (!context.Evaluated.Select(t => t.Value).All(t => t == true))
        {
            // Si no se valida correctamente, agregar el error a la lista de razones.
            return new(Responses.UnauthorizedByOrg)
            {
                Errors = [.. context.Reasons.Select(reason => new Types.Models.ErrorModel
                {
                    Tittle = "Acceso denegado",
                    Description = reason
                })]
            };
        }

        return new(Responses.Success);
    }


    /// <summary>
    /// Validar una sola política.
    /// </summary>
    private static bool ValidateSinglePolicyAsync(PolicyModel policy, PolicyValidatorContext context)
    {
        // Validar acceso por hora.
        if (context.Evaluated.ContainsKey("TIME") || !TimeAccessPolicyValidator.Validate(policy, context, policy.TimeAccessPolicies))
            return false;

        // Validar tipo de identidad
        if (context.Evaluated.ContainsKey("TYPE") || !IdentityTypePolicyValidator.Validate(policy, context, policy.IdentityTypePolicies))
            return false;

        return true;
    }

}