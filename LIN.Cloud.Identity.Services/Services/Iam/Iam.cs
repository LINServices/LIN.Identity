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


    /// <summary>
    /// Validar el nivel de acceso de una identidad sobre otra identidad.
    /// </summary>
    public async Task<IEnumerable<Roles>> IamIdentity(int identity1, int identity2)
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

        List<Roles> roles = new();

        foreach (var e in organizations)
        {
            var x = await Validate(identity1, e);
            roles.AddRange(x);
        }


        return roles;
    }


    /// <summary>
    /// Validar el nivel de acceso a una política.
    /// </summary>
    /// <param name="identity">Id de la identidad.</param>
    /// <param name="policy">Id de la política.</param>
    public async Task<IamLevels> IamPolicy(int identity, int policy)
    {

        // Obtener la identidad del dueño de la política.
        var ownerPolicy = await (from pol in context.Policies
                                 where pol.Id == policy
                                 select pol.Owner!.Directory.IdentityId).FirstOrDefaultAsync();

        // Obtener la organización.
        var organizationId = await groups.GetOwnerByIdentity(ownerPolicy);

        // Obtener roles de la identidad sobre la organización.
        var roles = await Validate(identity, organizationId.Model);

        // Tiene permisos para modificar la política.
        if (ValidateRoles.ValidateAlterPolicies(roles))
            return IamLevels.Privileged;

        return IamLevels.NotAccess;
    }

}