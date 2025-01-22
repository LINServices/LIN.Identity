using LIN.Cloud.Identity.Services.Auth.Interfaces;

namespace LIN.Cloud.Identity.Services.Auth;

public class AllowService(DataContext context) : IAllowService
{

    /// <summary>
    /// Validar si una lista de identidades puede acceder a una aplicación.
    /// </summary>
    /// <param name="identities">Ids de la identidad.</param>
    /// <param name="appId">Id de la aplicación.</param>
    public async Task<bool> IsAllow(IEnumerable<int> identities, int appId)
    {

        // Consulta.
        var isAllow = await (from allow in context.AllowApps
                             where allow.ApplicationId == appId
                             && identities.Contains(allow.Identity.Id)
                             select allow.IsAllow).ToListAsync();

        return isAllow.Contains(true);
    }

}