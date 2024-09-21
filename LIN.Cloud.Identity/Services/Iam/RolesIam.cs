using LIN.Cloud.Identity.Services.Utils;

namespace LIN.Cloud.Identity.Services.Iam;


public class RolesIam(DataContext context, IIdentityService identityService)
{


    /// <summary>
    /// Obtener los roles de una identidad en una organización.
    /// </summary>
    /// <param name="identity">Id de la identidad.</param>
    /// <param name="organization">Id de la organización.</param>
    public async Task<List<Roles>> RolesOn(int identity, int organization)
    {

        // Identidades.
        var identities = await identityService.GetIdentities(identity);

        // Obtener roles.
        var roles = await (from rol in context.IdentityRoles
                           where identities.Contains(rol.IdentityId)
                           && rol.OrganizationId == organization
                           select rol.Rol).ToListAsync();

        return roles;

    }


}




public static class ValidateRoles
{






    public static bool ValidateRead(IEnumerable<Roles> roles)
    {
        List<Roles> availed =
                    [
                        Roles.Administrator,
                        Roles.Manager,
                        Roles.AccountOperator,
                        Roles.Regular,
                        Roles.Viewer,
                        Roles.SecurityViewer
                    ];


        var sets = availed.Intersect(roles);

        return sets.Any();

    }


    public static bool ValidateAlterMembers(IEnumerable<Roles> roles)
    {
        List<Roles> availed =
                    [
                        Roles.Administrator,
                        Roles.Manager,
                        Roles.AccountOperator
                    ];

        var sets = availed.Intersect(roles);

        return sets.Any();

    }







}