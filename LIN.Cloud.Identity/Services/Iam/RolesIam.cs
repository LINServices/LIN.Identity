using LIN.Cloud.Identity.Services.Utils;
using LIN.Types.Enumerations;

namespace LIN.Cloud.Identity.Services.Iam;

public class RolesIam(DataContext context, Data.Groups groups, IIdentityService identityService)
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


    public async Task<IamLevels> IamPolicy(int identity, string policy)
    {

        // Is manager.
        var isOwner = await (from pol in context.Policies
                             where pol.OwnerIdentityId == identity
                             && pol.Id == Guid.Parse(policy)
                             select pol).AnyAsync();

        if (isOwner)
            return IamLevels.Privileged;

        // Validar en la organización.
        var olk = await (from pol in context.Policies
                         where pol.Id == Guid.Parse(policy)
                         select pol.OwnerIdentityId).FirstOrDefaultAsync();

        // Owner.
        var organizationId = await groups.GetOwnerByIdentity(olk);

        var roles = await RolesOn(identity, organizationId.Model);

        if (ValidateRoles.ValidateAlterMembers(roles))
            return IamLevels.Privileged;

        return IamLevels.NotAccess;

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

    public static bool ValidateInviteMembers(IEnumerable<Roles> roles)
    {
        List<Roles> availed =
                    [
                        Roles.Administrator,
                        Roles.Manager
                    ];

        var sets = availed.Intersect(roles);

        return sets.Any();

    }

    public static bool ValidateDelete(IEnumerable<Roles> roles)
    {
        List<Roles> availed =
                    [
                        Roles.Administrator,
                        Roles.Manager
                    ];

        var sets = availed.Intersect(roles);

        return sets.Any();

    }
}