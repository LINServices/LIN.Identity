namespace LIN.Identity.Data.Queries;

public class Directories
{



    /// <summary>
    /// Obtiene las identidades y directorios.
    /// </summary>
    /// <param name="identity">Identidad base</param>
    public static async Task<(List<int> directories, List<int> identities, List<DirectoryRoles> roles)> Get(int identity)
    {
        List<int> identities = [identity];
        List<int> directories = [];
        List<DirectoryRoles> roles = [];

        var (context, contextKey) = Conexión.GetOneConnection();

        await Get(identity, context, identities, directories, roles);

        context.CloseActions(contextKey);
        return (directories, identities, roles);
    }






















    /// <summary>
    /// Obtiene las identidades y directorios.
    /// </summary>
    /// <param name="identityBase">Identidad base</param>
    /// <param name="context">Contexto</param>
    /// <param name="identities">Lista de identidades.</param>
    /// <param name="directories">Directorios</param>
    /// <param name="roles">Roles</param>
    private static async Task Get(int identityBase, Conexión context, List<int> identities, List<int> directories, List<DirectoryRoles> roles)
    {
        // Consulta.
        var query = from DM in context.DataBase.DirectoryMembers
                    where DM.IdentityId == identityBase
                    && !identities.Contains(DM.Directory.IdentityId)
                    select new
                    {
                        Identity = DM.Directory.Identity.Id,
                        Directory = DM.Directory.ID,
                        Roles = DM.Identity.Roles.Select(x => x.Rol).DistinctBy(t => t)
                    };

        // Si hay elementos.
        if (query.Any())
        {
            var local = query.ToList();
            identities.AddRange(local.Select(t => t.Identity));
            directories.AddRange(local.Select(t => t.Directory));
            roles.AddRange(local.Select(t => t.Roles).SelectMany(t => t));

            foreach (var id in local)
                await Get(id.Identity, context, identities, directories, roles);

        }

    }


}