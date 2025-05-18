namespace LIN.Cloud.Identity.Services.Iam;

public class IamPolicy(DataContext context, IGroupRepository groups, IIamService rolesIam)
{

    /// <summary>
    /// Validar el nivel de acceso a una política.
    /// </summary>
    /// <param name="identity">Id de la identidad.</param>
    /// <param name="policy">Id de la política.</param>
    public async Task<IamLevels> Validate(int identity, int policy)
    {

        // Obtener la identidad del dueño de la política.
        var ownerPolicy = await (from pol in context.Policies
                                 where pol.Id == policy
                                 select pol.Owner.Directory.IdentityId).FirstOrDefaultAsync();

        // Obtener la organización.
        var organizationId = await groups.GetOwnerByIdentity(ownerPolicy);

        // Obtener roles de la identidad sobre la organización.
        var roles = await rolesIam.Validate(identity, organizationId.Model);

        // Tiene permisos para modificar la política.
        if (ValidateRoles.ValidateAlterPolicies(roles))
            return IamLevels.Privileged;

        return IamLevels.NotAccess;
    }

}