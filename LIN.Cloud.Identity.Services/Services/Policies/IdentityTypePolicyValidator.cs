using LIN.Cloud.Identity.Services.Models;
using LIN.Types.Cloud.Identity.Enumerations;
using LIN.Types.Cloud.Identity.Models;
using LIN.Types.Cloud.Identity.Models.Policies;

namespace LIN.Cloud.Identity.Services.Services.Policies;

internal class IdentityTypePolicyValidator
{

    /// <summary>
    /// Valida las políticas de tipo de identidad.
    /// </summary>
    public static bool Validate(PolicyModel policyBase, PolicyValidatorContext context, IEnumerable<IdentityTypePolicy> identityTypePolicies)
    {

        // Si no hay políticas de tipo de identidad, se permite el acceso.
        if (identityTypePolicies == null || !identityTypePolicies.Any())
            return true;

        // Tipo de identidad actual.
        IdentityType? identityType = context.AuthenticationRequest.Account?.Identity.Type;

        bool result = false;
        foreach (var e in identityTypePolicies)
            if (e.Type == identityType)
            {
                result = true; // Se encontró una política válida
                break;
            }

        bool isValid = (result && policyBase.Effect == PolicyEffect.Allow)
            || (!result && policyBase.Effect == PolicyEffect.Deny);


        context.Evaluated["TIPE"] = isValid;

        return isValid;
    }
}