namespace LIN.Cloud.Identity.Services.Auth;

public partial class Authentication
{

    /// <summary>
    /// Modelo de la aplicación.
    /// </summary>
    public ApplicationModel? Application { get; set; } = null;


    /// <summary>
    /// Validar aplicación.
    /// </summary>
    private async Task<bool> ValidateApp()
    {

        // Obtener la restrictions.
        var appResponse = await applications.Read(AppCode);
        Application = appResponse.Model;

        // Si no se requiere validar la restrictions.
        if (Account!.IsLINAdmin || !Settings.ValidateApp)
            return true;

        // Validar si la restrictions existe.
        if (appResponse.Response != Responses.Success)
            return false;

        // Obtener las restricciones.
        var restriction = await applicationRestrictions.Read(AppCode);

        // Validar las restricciones.
        var isAuthorized = await ValidateRestrictions(restriction.Model);

        // Respuesta.
        return isAuthorized;
    }


    /// <summary>
    /// Validar restricciones.
    /// </summary>
    /// <param name="restriction">Modelo de las restricciones.</param>
    private async Task<bool> ValidateRestrictions(ApplicationRestrictionModel? restriction)
    {

        // Si no hay.
        if (restriction == null)
            return true;

        // Validar por el tipo de cuenta.
        switch (Account!.AccountType)
        {
            case AccountTypes.Personal:
                if (!restriction.AllowPersonalAccounts) return false;
                break;
            case AccountTypes.Work:
                if (!restriction.AllowWorkAccounts) return false;
                break;
            case AccountTypes.Education:
                if (!restriction.AllowEducationsAccounts) return false;
                break;
        }

        // Restricciones de tiempo.
        if (restriction.RestrictedByTime)
        {
            bool isAuthorized = await ValidateRestrictionsTimes();
            if (!isAuthorized)
                return false;
        }

        // Restricciones de identidad.
        if (restriction.RestrictedByIdentities)
        {
            bool isAuthorized = await ValidateRestrictionsIdentity();
            if (!isAuthorized)
                return false;
        }

        return true;
    }


    /// <summary>
    /// Validar restricciones de tiempo.
    /// </summary>
    private async Task<bool> ValidateRestrictionsTimes()
    {
        // Hora actual.
        var now = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);

        // Obtener las restricciones de tiempo.
        var times = await applicationRestrictions.ReadTimes(Application!.Id);

        // Validar si alguna encaja.
        foreach (var time in times.Models)
        {
            if (now > time.StartTime && now < time.EndTime)
                return true;
        }

        return false;
    }


    /// <summary>
    /// Validar restricciones de identidades.
    /// </summary>
    private async Task<bool> ValidateRestrictionsIdentity()
    {

        // Obtener las identidades de un usuario.
        var identities = await identityService.GetIdentities(Account!.IdentityId);

        // Validar si alguna esta autorizada para acceder a la app.
        bool isAllow = await allowService.IsAllow(identities, Application!.Id);

        return isAllow;
    }

}