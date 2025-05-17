using LIN.Types.Cloud.Identity.Models;
using LIN.Types.Cloud.Identity.Models.Policies;

namespace LIN.Cloud.Identity.Services.Services.Policies;

internal class TimeAccessPolicyValidator
{

    /// <summary>
    /// Valida las políticas de acceso por tiempo.
    /// </summary>
    /// <param name="context">Contexto actual.</param>
    /// <param name="policies">Lista de políticas.</param>
    public static bool Validate(PolicyModel policyBase, PolicyValidatorContext context, IEnumerable<TimeAccessPolicy> policies)
    {
        if (!policies.Any())
            return true;

        // Hora actual.
        var now = TimeOnly.FromDateTime(DateTime.UtcNow);

        bool result = false;
        foreach (var policy in policies)
        {
            if (now >= policy.StartHour && now <= policy.EndHour)
            {
                result = true; // Se encontró una política válida
                break;
            }
        }

        bool isValid = (result && policyBase.Effect == PolicyEffect.Allow)
                    || (!result && policyBase.Effect == PolicyEffect.Deny);

        context.Evaluated["TIME"] = isValid;
        return true;
    }
}