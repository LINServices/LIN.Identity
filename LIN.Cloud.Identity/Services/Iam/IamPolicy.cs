namespace LIN.Cloud.Identity.Services.Iam;

public class IamPolicy(DataContext context, Data.Groups groups, IamRoles rolesIam)
{

    /// <summary>
    /// Validar el nivel de acceso a una política.
    /// </summary>
    /// <param name="identity">Id de la identidad.</param>
    /// <param name="policy">Id de la política.</param>
    public async Task<IamLevels> Validate(int identity, string policy)
    {

        // Si la identidad es la administradora de la política.
        var isOwner = await (from pol in context.Policies
                             where pol.OwnerIdentityId == identity
                             && pol.Id == Guid.Parse(policy)
                             select pol).AnyAsync();

        // Es privilegiado.
        if (isOwner)
            return IamLevels.Privileged;

        // Obtener la identidad del dueño de la política.
        var olk = await (from pol in context.Policies
                         where pol.Id == Guid.Parse(policy)
                         select pol.OwnerIdentityId).FirstOrDefaultAsync();

        // Obtener la organización.
        var organizationId = await groups.GetOwnerByIdentity(olk);

        // Obtener roles de la identidad sobre la organización.
        var roles = await rolesIam.Validate(identity, organizationId.Model);

        // Tiene permisos para modificar la política.
        if (ValidateRoles.ValidateAlterPolicies(roles))
            return IamLevels.Privileged;

        return IamLevels.NotAccess;
    }

}