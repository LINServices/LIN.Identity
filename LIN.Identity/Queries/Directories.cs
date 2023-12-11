namespace LIN.Identity.Queries;

public class Directories
{



    public static async Task<List<int>> GetDirectories(int identidad)
    {
        List<int> identities = [];
        List<int> directories = [];

        var (context, contextKey) = Conexión.GetOneConnection();

        await GetDirectories(identidad, identities, context, directories);

        context.CloseActions(contextKey);
        return directories;
    }



    public static async Task GetDirectories(int identidad, List<int> final, Conexión context, List<int> directories)
    {


        // Consulta.
        var query = from DM in context.DataBase.DirectoryMembers
                    where DM.IdentityId == identidad
                    && !final.Contains(DM.Directory.IdentityId)
                    select new
                    {
                        Identity = DM.Directory.Identity.Id,
                        Directory = DM.Directory.ID
                    };

        // Si hay elementos.
        if (query.Any())
        {
            var local = query.ToList();
            final.AddRange(local.Select(t => t.Identity));
            directories.AddRange(local.Select(t => t.Directory));

            foreach (var id in local)
                await GetDirectories(id.Identity, final, context, directories);

        }

    }

}
