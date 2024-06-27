namespace LIN.Cloud.Identity.Services.Iam;


public class RolesIam(DataContext context)
{




    public async Task<(List<int> identities, List<Roles> roles)> RolesOnByDir(int identity, int directory)
    {

        var query = await (from org in context.Organizations
                           where org.DirectoryId == directory
                           select org.Id).FirstOrDefaultAsync();


        if (query <= 0)
            return ([], []);



        List<int> identities = [identity];
        List<Types.Cloud.Identity.Enumerations.Roles> roles = [];

        await RolesOn(identity, query, identities, roles);

        return (identities, roles);
    }



    /// <summary>
    /// Obtener los roles directos y heredades de una identidad en una organización.
    /// </summary>
    /// <param name="identity">Id de la identidad.</param>
    /// <param name="organization">Id de la organización.</param>
    public async Task<(List<int> identities, List<Roles> roles)> RolesOn(int identity, int organization)
    {
        List<int> identities = [identity];
        List<Types.Cloud.Identity.Enumerations.Roles> roles = [];

        await RolesOn(identity, organization, identities, roles);

        return (identities, roles);
    }



    /// <summary>
    /// Obtener los roles.
    /// </summary>
    /// <param name="identity">Id de la identidad.</param>
    /// <param name="organization">Id de la organización.</param>
    /// <param name="ids">Lista de Ids de identidades asociadas.</param>
    /// <param name="roles">Lista de roles asociados.</param>
    private async Task RolesOn(int identity, int organization, List<int> ids, List<Roles> roles)
    {

        // Consulta.
        var query = from id in context.Identities
                    where id.Id == identity
                    select new
                    {
                        // Encontrar grupos donde la identidad pertenece.
                        In = (from member in context.GroupMembers
                              where !ids.Contains(member.Group.IdentityId)
                              && member.IdentityId == identity
                              select member.Group.IdentityId).ToList(),

                        // Obtener roles.
                        Roles = (from IR in context.IdentityRoles
                                 where IR.IdentityId == identity
                                 && IR.OrganizationId == organization
                                 select IR.Rol).ToList()
                    };


        // Si hay elementos.
        if (query.Any())
        {

            // Ejecuta la consulta.
            var local = query.ToList();

            // Obtiene los roles.
            var localRoles = local.SelectMany(t => t.Roles);

            // Obtiene las bases.
            var bases = local.SelectMany(t => t.In);

            // Agregar a los objetos.
            roles.AddRange(localRoles);
            ids.AddRange(bases);

            // Recorrer.
            foreach (var @base in bases)
                await RolesOn(@base, organization, ids, roles);

        }

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