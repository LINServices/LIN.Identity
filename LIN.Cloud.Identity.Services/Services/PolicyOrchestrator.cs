using LIN.Cloud.Identity.Persistence.Repositories.Mongo;
using LIN.Cloud.Identity.Services.Services.Policies;
using LIN.Types.Cloud.Identity.Models.Policies;

namespace LIN.Cloud.Identity.Services.Services;

internal class PolicyOrchestrator(PolicyRepository policyRepository, IIdentityService identityService) : IPolicyOrchestrator
{

    /// <summary>
    /// Valida las políticas de acceso para una identidad (incluyendo su jerarquía).
    /// </summary>
    public async Task<ReadOneResponse<PolicyValidatorContext>> ValidatePoliciesForOrganization(AuthenticationRequest request)
    {
        // 1. Obtener los niveles de identidad (0=Usuario, 1=Grupo...).
        var levels = await identityService.GetLevel(request.Account!.Identity.Id, request.Account.Identity.OwnerId!.Value);

        // Contexto de evaluación.
        var evaluationContext = new PolicyEvaluationContext
        {
            IdentityType = request.Account.Identity.Type,
            CurrentTime = DateTime.UtcNow
        };

        // 2. Obtener todas las políticas de la jerarquía agrupadas por nivel.
        var policiesByLevel = new Dictionary<int, List<AccessPolicyModel>>();
        foreach (var level in levels.OrderBy(t => t.Key))
        {
            var ids = level.Value.Select(t => t.Identity).ToList();
            policiesByLevel[level.Key] = await policyRepository.ReadAll(ids);
        }

        var deniedPolicies = new List<AccessPolicyModel>();

        // 3. Evaluar cada categoría de forma independiente a través de la jerarquía.

        // --- TIEMPO ---
        var timeResult = EvaluateCategoryPrecedence(policiesByLevel, p => PolicyValidator.HasTimeRules(p), p => PolicyValidator.EvaluateTimeMatch(p, evaluationContext.CurrentTime));
        if (timeResult.Denied)
            deniedPolicies.AddRange(timeResult.Policies);

        // --- IDENTIDAD ---
        var identityResult = EvaluateCategoryPrecedence(policiesByLevel, p => PolicyValidator.HasIdentityRules(p), p => PolicyValidator.EvaluateIdentityMatch(p, evaluationContext.IdentityType));
        if (identityResult.Denied)
            deniedPolicies.AddRange(identityResult.Policies);

        // --- RED ---
        var networkResult = EvaluateCategoryPrecedence(policiesByLevel, p => PolicyValidator.HasNetworkRules(p), p => PolicyValidator.EvaluateNetworkMatch(p, evaluationContext));
        if (networkResult.Denied)
            deniedPolicies.AddRange(networkResult.Policies);

        // 4. Reportar errores si hubo denegaciones.
        if (deniedPolicies.Any())
        {
            return new(Responses.UnauthorizedByOrg)
            {
                Message = "Acceso denegado por política de seguridad.",
                Errors = deniedPolicies.DistinctBy(t => t.Id).Select(p => new LIN.Types.Models.ErrorModel
                {
                    Code = p.Id.ToString(),
                    Tittle = "Política bloqueada",
                    Description = $"La política '{p.Name}' bloqueó el acceso o no se cumplieron sus requisitos."
                }).ToList()
            };
        }

        return new(Responses.Success);
    }


    /// <summary>
    /// Evalúa la precedencia de una categoría de reglas a través de los niveles.
    /// </summary>
    private static (bool Denied, List<AccessPolicyModel> Policies) EvaluateCategoryPrecedence(
        Dictionary<int, List<AccessPolicyModel>> policiesByLevel,
        Func<AccessPolicyModel, bool> hasCategoryRules,
        Func<AccessPolicyModel, bool> isMatch)
    {
        bool anyPolicyExisted = false;

        // Recorrer niveles (0=Usuario, 1=Grupo...)
        foreach (var level in policiesByLevel.OrderBy(t => t.Key))
        {
            var categoryPolicies = level.Value.Where(hasCategoryRules).ToList();
            if (!categoryPolicies.Any()) continue;

            anyPolicyExisted = true;

            // En este nivel, buscar si alguna política coincide con el contexto.
            var matchingPolicies = categoryPolicies.Where(isMatch).ToList();

            if (matchingPolicies.Any())
            {
                // Si hay coincidencias en este nivel, la decisión se toma AQUÍ.
                // Deny tiene precedencia sobre Allow en el mismo nivel.
                var denyPolicies = matchingPolicies.Where(t => t.Effect == PolicyEffect.Deny).ToList();
                if (denyPolicies.Any())
                    return (true, denyPolicies);

                // Si no hay Deny pero hay Allow, está permitido para esta categoría.
                return (false, []);
            }

            // Si ninguna política de este nivel coincide (NotMatch), continuamos al siguiente nivel de la jerarquía.
        }

        // Si recorrimos toda la jerarquía y hubo políticas pero NINGUNA coincidió con el contexto:
        if (anyPolicyExisted)
        {
            // Bloqueamos porque había requisitos y no se cumplió ninguno.
            // Usamos las políticas del nivel más cercano para informar el error.
            var firstAuthPolicies = policiesByLevel.OrderBy(t => t.Key).First(t => t.Value.Any(hasCategoryRules)).Value.Where(hasCategoryRules).ToList();
            return (true, firstAuthPolicies);
        }

        // Si no existen políticas para esta categoría en ningún nivel, por defecto es OK.
        return (false, []);
    }


    /// <summary>
    /// Valida las políticas de acceso para una aplicación.
    /// </summary>
    public async Task<ReadOneResponse<PolicyValidatorContext>> ValidatePoliciesForApplication(AuthenticationRequest request, int appId)
    {
        // NOTA: Este método debería seguir una lógica similar si las aplicaciones también tienen políticas jerárquicas.
        // Por ahora, se asume que las políticas de aplicación son directas.

        // CAMBIAR PORQUE ES POR APP NO PO ID
        var policies = await policyRepository.ReadAll(appId);

        var evaluationContext = new PolicyEvaluationContext
        {
            IdentityType = request.Account!.Identity.Type,
        };

        var evaluations = policies.Models.Select(p => new
        {
            Policy = p,
            Decision = PolicyValidator.Evaluate(p, evaluationContext)
        }).ToList();

        if (evaluations.Any(t => t.Decision == PolicyDecision.Deny))
        {
            var denied = evaluations.Where(t => t.Decision == PolicyDecision.Deny).Select(t => t.Policy).ToList();

            return new(Responses.UnauthorizedByApp)
            {
                Message = "La aplicación ha denegado el acceso.",
                Errors = denied.Select(p => new LIN.Types.Models.ErrorModel
                {
                    Code = p.Id.ToString(),
                    Tittle = "Política de aplicación bloqueada",
                    Description = $"La política '{p.Name}' bloqueó el acceso a la aplicación."
                }).ToList()
            };
        }

        if (evaluations.Any(t => t.Decision == PolicyDecision.Allow))
            return new(Responses.Success);

        return new(Responses.Success);
    }

}