using LIN.Types.Cloud.Identity.Models.Policies;

namespace LIN.Cloud.Identity.Services.Services.Policies;

/// <summary>
/// Validador unificado de políticas de acceso.
/// </summary>
public static class PolicyValidator
{
    /// <summary>
    /// Verifica si una política tiene reglas de un tipo específico.
    /// </summary>
    public static bool HasTimeRules(AccessPolicyModel policy) => policy.TimeRules.Any();
    public static bool HasIdentityRules(AccessPolicyModel policy) => policy.IdentityTypeRules.Any();
    public static bool HasNetworkRules(AccessPolicyModel policy) => policy.NetworkRules.Any();


    /// <summary>
    /// Evalúa la coincidencia de las reglas de tiempo de una política.
    /// </summary>
    public static bool EvaluateTimeMatch(AccessPolicyModel policy, DateTime now)
        => !policy.TimeRules.Any() || policy.TimeRules.Any(r => IsTimeMatch(r, now));


    /// <summary>
    /// Evalúa la coincidencia de las reglas de tipo de identidad de una política.
    /// </summary>
    public static bool EvaluateIdentityMatch(AccessPolicyModel policy, IdentityType type)
        => !policy.IdentityTypeRules.Any() || policy.IdentityTypeRules.Any(r => IsIdentityTypeMatch(r, type));


    /// <summary>
    /// Evalúa la coincidencia de las reglas de red de una política.
    /// </summary>
    public static bool EvaluateNetworkMatch(AccessPolicyModel policy, PolicyEvaluationContext context)
        => !policy.NetworkRules.Any() || IsNetworkMatch(policy.NetworkRules, context);


    /// <summary>
    /// Evalúa una política contra el contexto actual.
    /// </summary>
    public static PolicyDecision Evaluate(AccessPolicyModel policy, PolicyEvaluationContext context)
    {
        // Verificar si se cumplen las reglas de la política.
        bool matchesTime = EvaluateTimeMatch(policy, context.CurrentTime);
        bool matchesIdentity = EvaluateIdentityMatch(policy, context.IdentityType);
        bool matchesNetwork = EvaluateNetworkMatch(policy, context);

        bool allRulesPassed = matchesTime && matchesIdentity && matchesNetwork;

        // Si no se cumplen las condiciones de la política.
        if (!allRulesPassed)
        {
            // Si la política era para PERMITIR (Whitelist), y no cumplo, bloqueo el acceso.
            // Si la política era para DENEGAR (Blacklist), y no cumplo, no pasa nada (sigo evaluando).
            return policy.Effect == PolicyEffect.Allow ? PolicyDecision.Deny : PolicyDecision.NotMatch;
        }

        // Si se cumplen todas las condiciones (o no había reglas).
        // Se aplica el efecto directo de la política.
        return policy.Effect == PolicyEffect.Allow ? PolicyDecision.Allow : PolicyDecision.Deny;
    }


    /// <summary>
    /// Valida si la hora actual coincide con una regla.
    /// </summary>
    private static bool IsTimeMatch(TimeRule rule, DateTime now)
    {
        // Validar día.
        if (rule.Days.Any() && !rule.Days.Contains(now.DayOfWeek))
            return false;

        var currentTime = TimeOnly.FromDateTime(now);

        // Validar rango horario.
        return currentTime >= rule.StartTime && currentTime <= rule.EndTime;
    }


    /// <summary>
    /// Valida si el tipo de identidad coincide.
    /// </summary>
    private static bool IsIdentityTypeMatch(IdentityTypeRule rule, IdentityType currentType)
    {
        return rule.AllowedTypes.Contains(currentType);
    }


    /// <summary>
    /// Valida reglas de red.
    /// </summary>
    private static bool IsNetworkMatch(List<NetworkRule> rules, PolicyEvaluationContext context)
    {
        foreach (var rule in rules)
        {
            // 1. Si está en la lista negra de IP, esta regla no se cumple para este contexto.
            if (rule.IPBlacklist.Any() && rule.IPBlacklist.Contains(context.IPAddress))
                continue;

            // 2. Si hay una lista blanca de IP y no estás en ella, esta regla no se cumple.
            if (rule.IPWhitelist.Any() && !rule.IPWhitelist.Contains(context.IPAddress))
                continue;

            // 3. Si hay lista de países permitidos y no estás en ella, esta regla no se cumple.
            if (rule.AllowedCountries.Any() && !rule.AllowedCountries.Contains(context.CountryCode))
                continue;

            // Si superó los filtros anteriores, el contexto "cae" dentro de esta regla.
            return true;
        }

        return false;
    }
}


/// <summary>
/// Contexto de evaluación para las políticas.
/// </summary>
public record PolicyEvaluationContext
{
    public DateTime CurrentTime { get; init; } = DateTime.UtcNow;
    public IdentityType IdentityType { get; init; }
    public string IPAddress { get; init; } = string.Empty;
    public string CountryCode { get; init; } = string.Empty;
}


/// <summary>
/// Resultado de la evaluación de una política.
/// </summary>
public enum PolicyDecision
{
    Allow,
    Deny,
    NotMatch
}
