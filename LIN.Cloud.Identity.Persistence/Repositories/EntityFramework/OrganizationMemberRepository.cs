namespace LIN.Cloud.Identity.Persistence.Repositories.EntityFramework;

internal class OrganizationMemberRepository(DataContext context) : IOrganizationMemberRepository
{

    /// <summary>
    /// Valida si una identidad es miembro de una organización.
    /// </summary>
    /// <param name="id">Identidad.</param>
    /// <param name="organization">Id de la organización.</param>
    public async Task<ReadOneResponse<GroupMemberTypes>> IamIn(int id, int organization)
    {
        try
        {
            // Consulta.
            var query = await (from org in context.Organizations
                               where org.Id == organization
                               join gm in context.GroupMembers
                               on org.DirectoryId equals gm.GroupId
                               where gm.IdentityId == id
                               select new
                               {
                                   gm.Type
                               }).FirstOrDefaultAsync();

            // Si la cuenta no existe.
            if (query is null)
            {

                var directory = await (from A in context.Organizations
                                       where A.Directory.IdentityId == id
                                       && A.Id == organization
                                       select A).AnyAsync();

                if (!directory)
                    return new(Responses.NotRows);
            }


            // Success.
            return new(Responses.Success, query?.Type ?? GroupMemberTypes.Group);
        }
        catch (Exception)
        {
            return new();
        }

    }


    /// <summary>
    /// Expulsar identidades de la organización.
    /// </summary>
    /// <param name="ids">Lista de identidades.</param>
    /// <param name="organization">Id de la organización.</param>
    /// <returns>Respuesta del proceso.</returns>
    public async Task<ResponseBase> Expulse(IEnumerable<int> ids, int organization)
    {
        try
        {
            // Desactivar identidades (Solo creadas dentro de la propia organización).
            var baseQuery = (from member in context.GroupMembers
                             where ids.Contains(member.IdentityId)
                             join org in context.Organizations
                             on member.Identity.OwnerId equals org.Id
                             where org.Id == organization
                             select member);

            // Desactivar identidades (Solo creadas dentro de la propia organización).
            await baseQuery.Where(m => m.Type != GroupMemberTypes.Guest).Select(m => m.Identity).ExecuteUpdateAsync(t => t.SetProperty(t => t.Status, IdentityStatus.Disable));

            // Eliminar accesos (Tanto propios de la organización como los externos).
            await baseQuery.ExecuteDeleteAsync();

            // Eliminar roles asociados.
            await (from rol in context.IdentityRoles
                   where ids.Contains(rol.IdentityId)
                   && rol.OrganizationId == organization
                   select rol).ExecuteDeleteAsync();

            // Success.
            return new(Responses.Success);
        }
        catch (Exception)
        {
            return new();
        }

    }


    /// <summary>
    /// Obtener los integrantes de una organización.
    /// </summary>
    /// <param name="id">Id de la organización.</param>
    public async Task<ReadAllResponse<OrganizationModel>> ReadAll(int id)
    {
        try
        {
            // Consulta.
            var query = await (from org in context.Organizations
                               join gm in context.GroupMembers
                               on org.DirectoryId equals gm.GroupId
                               where gm.IdentityId == id
                               select org).ToListAsync();

            // Success.
            return new(Responses.Success, query);
        }
        catch (Exception)
        {
            return new();
        }
    }

}