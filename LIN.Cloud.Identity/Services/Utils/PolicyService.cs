using Newtonsoft.Json.Linq;

namespace LIN.Cloud.Identity.Services.Utils;

public class PolicyService(IamPolicy iamPolicy)
{



    public (bool valid, string message) Validate(IEnumerable<PolicyRequirementModel> requirements)
    {

        bool isValid = Time(requirements);

        if (!isValid)
            return (false, "No tienes acceso a la política por la hora actual.");

        // Validar contaseña.
        isValid = PasswordTime(requirements);

        if (!isValid)
            return (false, "No tienes acceso a la política devido a que no has cambiado la contraseña en los últimos N dias.");

        isValid = TFA(requirements);

        if (!isValid)
            return (false, "No tienes acceso a la política debido a que no tienes el doble factor de autenticación");

        return (true, string.Empty);

    }


    bool Time(IEnumerable<PolicyRequirementModel> requirements)
    {

        // Solo de tiempo.
        requirements = requirements.Where(x => x.Type == PolicyRequirementTypes.Time);

        // Si no hay políticas de tiempo.
        if (!requirements.Any())
            return true;

        // Validar los requerimientos de tiempo.
        foreach (var requirement in requirements)
        {

            // Objeto.
            JObject jsonObject = JObject.Parse(requirement.Requirement ?? "");

            // Obtener los ticks.
            var startTicks = Convert.ToInt64(jsonObject["start"]);
            var endTicks = Convert.ToInt64(jsonObject["end"]);

            // Parsear a tiempo.
            var start = TimeSpan.FromTicks(startTicks);
            var end = TimeSpan.FromTicks(endTicks);

            // var days = TimeSpan.FromTicks((requirement.Requirement as dynamic).days);

            bool valid = iamPolicy.PolicyRequirement(start, end);

            if (valid)
                return true;


        }

        return false;
    }


    bool PasswordTime(IEnumerable<PolicyRequirementModel> requirements)
    {

        // Solo de tiempo.
        requirements = requirements.Where(x => x.Type == PolicyRequirementTypes.PasswordTime);

        // Si no hay políticas de tiempo.
        if (!requirements.Any())
            return true;

        // Validar los requerimientos de tiempo.
        foreach (var requirement in requirements)
        {

            // Objeto.
            JObject jsonObject = JObject.Parse(requirement.Requirement ?? "");

            // Obtener los ticks.
            var days = Convert.ToInt64(jsonObject["days"]);


            // Dias.
            if (days <= 30)
            {
                return true;
            }

        }

        return false;
    }


    bool TFA(IEnumerable<PolicyRequirementModel> requirements)
    {

        // Solo de tiempo.
        requirements = requirements.Where(x => x.Type == PolicyRequirementTypes.TFA);

        // Si no hay políticas de tiempo.
        if (!requirements.Any())
            return true;

        // Validar doble facto.
        return false;
    }



}