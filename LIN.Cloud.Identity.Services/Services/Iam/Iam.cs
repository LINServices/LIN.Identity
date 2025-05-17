using LIN.Cloud.Identity.Persistence.Contexts;
using LIN.Cloud.Identity.Services.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LIN.Cloud.Identity.Services.Services.Iam;

internal class IamService(DataContext context, IGroupRepository groups, IIdentityService identityService) : IIamService
{

    /// <summary>
    /// Obtener los roles de una identidad en una organización.
    /// </summary>
    /// <param name="identity">Id de la identidad.</param>
    /// <param name="organization">Id de la organización.</param>
    public async Task<IEnumerable<Roles>> Validate(int identity, int organization)
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


    public async Task<IamLevels> IamIdentity(int identity1, int identity2)
    {

        var organizations = await (from z in context.Groups
                                   where z.Members.Any(x => x.IdentityId == identity1)
                                   && z.Members.Any(x => x.IdentityId == identity2)
                                   select z.Identity.Owner!.Id).Distinct().ToListAsync();

        // Si es un grupo.
        var organization = await groups.GetOwnerByIdentity(identity2);

        if (organization.Response == Responses.Success)
        {
            organizations.Add(organization.Model);
            organizations = organizations.Distinct().ToList();
        }

        bool have = false;

        foreach (var e in organizations)
        {
            var x = await Validate(identity1, e);

            if (x.ValidateReadSecure())
            {
                have = true;
                break;
            }
        }


        return have ? IamLevels.Privileged : IamLevels.NotAccess;
    }

}