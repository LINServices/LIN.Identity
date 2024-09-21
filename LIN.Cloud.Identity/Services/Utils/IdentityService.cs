namespace LIN.Cloud.Identity.Services.Utils;


public class IdentityService(DataContext context) : IIdentityService
{

    public async Task<List<int>> GetIdentities(int identity)
    {
        List<int> result = [identity];
        await GetIdentities(identity, result);
        return result;
    }

    private async Task GetIdentities(int identity, List<int> ids)
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
                    };


        // Si hay elementos.
        if (query.Any())
        {
            // Ejecuta la consulta.
            var local = query.ToList();

            // Obtiene las bases.
            var bases = local.SelectMany(t => t.In);

            // Agregar a los objetos.
            ids.AddRange(bases);

            // Recorrer.
            foreach (var @base in bases)
                await GetIdentities(@base, ids);

        }
    }

}