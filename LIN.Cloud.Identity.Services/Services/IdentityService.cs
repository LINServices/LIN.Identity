using LIN.Cloud.Identity.Persistence.Contexts;

namespace LIN.Cloud.Identity.Services.Services;

internal class IdentityService(DataContext context) : IIdentityService
{

    /// <summary>
    /// Obtener la identidades asociadas a una identidad base.
    /// </summary>
    /// <param name="identity">Identidad base.</param>
    public async Task<List<int>> GetIdentities(int identity)
    {
        List<int> result = [identity];
        await GetIdentities(identity, result);
        return result;
    }


    /// <summary>
    /// Obtener la identidades asociadas a una identidad base.
    /// </summary>
    /// <param name="identity">Identidad base.</param>
    public async Task<Dictionary<int, List<IdentityLevelModel>>> GetLevel(int identity, int organization)
    {
        Dictionary<int, List<IdentityLevelModel>> result = new()
        {
            {
                0,
                [
                    new() {
                        Identity = identity,
                        Level = 0
                }]
            }
        };

        await GetIdentities(identity, 1, organization, [], result);
        return result;
    }


    /// <summary>
    /// Obtener la identidades asociadas a una identidad base.
    /// </summary>
    /// <param name="identity">Identidad base.</param>
    /// <param name="ids">Identidades encontradas.</param>
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

        // Si no hay elementos.
        if (!query.Any())
            return;

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


    /// <summary>
    /// Obtener la identidades asociadas a una identidad base.
    /// </summary>
    /// <param name="identity">Identidad base.</param>
    /// <param name="ids">Identidades encontradas.</param>
    private async Task GetIdentities(int identity, int level, int organization, List<int> ids, Dictionary<int, List<IdentityLevelModel>> keys)
    {
        // Consulta.
        var query = from id in context.Identities
                    where id.Id == identity
                    && id.OwnerId == organization
                    select new
                    {
                        // Encontrar grupos donde la identidad pertenece.
                        In = (from member in context.GroupMembers
                              where !ids.Contains(member.Group.IdentityId)
                              && member.IdentityId == identity
                              select member.Group.IdentityId).ToList(),
                    };

        // Si no hay elementos.
        if (!query.Any())
            return;

        // Ejecuta la consulta.
        var local = query.ToList();

        // Obtiene las bases.
        var bases = local.SelectMany(t => t.In);

        // Agregar a los objetos.
        ids.AddRange(bases.Select(t => t));

        keys.TryGetValue(level, out var list);

        if (list == null)
            keys.Add(level, bases.Select(t => new IdentityLevelModel
            {
                Identity = t,
                Level = level
            }).ToList());
        else
            list.AddRange(bases.Select(t => new IdentityLevelModel
            {
                Identity = t,
                Level = level
            }));

        // Recorrer.
        foreach (var @base in bases)
            await GetIdentities(@base, level + 1, organization, ids, keys);

    }

}


class IdentityLevelModel
{
    public int Level { get; set; }
    public int Identity { get; set; }
}