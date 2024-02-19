namespace LIN.Cloud.Identity.Services.Iam;


public class RolesIam
{




    public static async Task<(List<int> identities, List<Types.Cloud.Identity.Enumerations.Roles> roles)> RolesOnByDir(int identity, int directory)
    {

        var (context, contextKey) = DataService.GetConnection();

        var query = await (from org in context.Organizations
                           where org.DirectoryId == directory
                           select org.Id).FirstOrDefaultAsync();


        if (query <= 0)
            return ([], []);



        List<int> identities = [identity];
        List<Types.Cloud.Identity.Enumerations.Roles> roles = [];

        await RolesOn(identity, query, context, identities, roles);

        context.Close(contextKey);
        return (identities, roles);
    }




    public static async Task<(List<int> identities, List<Types.Cloud.Identity.Enumerations.Roles> roles)> RolesOn(int identity, int organization)
    {
        List<int> identities = [identity];
        List<Types.Cloud.Identity.Enumerations.Roles> roles = [];

        var (context, contextKey) = DataService.GetConnection();

        await RolesOn(identity, organization, context, identities, roles);

        context.Close(contextKey);
        return (identities, roles);
    }


    private static async Task RolesOn(int identity, int organization, DataContext context, List<int> ids, List<Types.Cloud.Identity.Enumerations.Roles> roles)
    {


        var query = from id in context.Identities
                    where id.Id == identity
                    select new
                    {
                        In = (from member in context.GroupMembers
                              where !ids.Contains(member.Group.IdentityId)
                              && member.IdentityId == identity
                              select member.Group.IdentityId).ToList(),

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
                await RolesOn(@base, organization, context, ids, roles);

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