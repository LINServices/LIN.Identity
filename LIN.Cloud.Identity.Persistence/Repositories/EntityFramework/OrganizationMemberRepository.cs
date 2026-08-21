using LIN.Types.Cloud.Identity.Abstracts;

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
            var query = await (from org in context.Organizations
                               where org.Id == organization
                               join gm in context.GroupMembers
                               on org.DirectoryId equals gm.GroupId
                               where gm.IdentityId == id
                               select new
                               {
                                   gm.Type
                               }).FirstOrDefaultAsync();

            if (query is null)
            {

                var directory = await (from A in context.Organizations
                                       where A.Directory.IdentityId == id
                                       && A.Id == organization
                                       select A).AnyAsync();

                if (!directory)
                    return new(Responses.NotRows);
            }

            return new(Responses.Success, query?.Type ?? GroupMemberTypes.Group);
        }
        catch (Exception)
        {
            return new();
        }

    }


    /// <summary>
    /// Valida si una lista de identidades son miembro de una organización.
    /// </summary>
    /// <param name="ids">Identidades</param>
    /// <param name="organization">Id de la organización</param>
    public async Task<(IEnumerable<int> success, List<int> failure)> IamIn(IEnumerable<int> ids, int organization)
    {

        try
        {

            var query = await (from org in context.Organizations
                               where org.Id == organization
                               join gm in context.GroupMembers
                               on org.DirectoryId equals gm.GroupId
                               where ids.Contains(gm.IdentityId)
                               select gm.IdentityId).ToListAsync();

            List<int> success = [.. query];
            List<int> failure = [.. ids.Except(success)];

            return (success, failure);
        }
        catch (Exception)
        {
        }
        return ([], []);
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
            var baseQuery = (from member in context.GroupMembers
                             where ids.Contains(member.IdentityId)
                             join org in context.Organizations
                             on member.Identity.OwnerId equals org.Id
                             where org.Id == organization
                             select member);

            // Solo se deshabilitan las identidades creadas dentro de la propia organización; los invitados (Guest) conservan su estado.
            await baseQuery.Where(m => m.Type != GroupMemberTypes.Guest).Select(m => m.Identity).ExecuteUpdateAsync(t => t.SetProperty(t => t.Status, IdentityStatus.Disable));

            // Se eliminan tanto los accesos propios de la organización como los externos (invitados).
            await baseQuery.ExecuteDeleteAsync();

            await (from rol in context.IdentityRoles
                   where ids.Contains(rol.IdentityId)
                   && rol.OrganizationId == organization
                   select rol).ExecuteDeleteAsync();

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
    public async Task<ReadAllResponse<GroupMember>> ReadAll(int id)
    {
        try
        {
            var query = await (from gm in context.GroupMembers
                               where gm.Group.Identity.OwnerId == id
                               select gm).ToListAsync();

            return new(Responses.Success, query);
        }
        catch (Exception)
        {
            return new();
        }
    }


    /// <summary>
    /// Obtener las organizaciones donde una identidad es integrante
    /// </summary>
    /// <param name="identity">Id de la identidad.</param>
    public async Task<ReadAllResponse<OrganizationModel>> ReadAllMembers(int identity)
    {
        try
        {
            var query = await (from org in context.Organizations
                               join gm in context.GroupMembers
                               on org.DirectoryId equals gm.GroupId
                               where gm.IdentityId == identity
                               select org).ToListAsync();

            return new(Responses.Success, query);
        }
        catch (Exception)
        {
            return new();
        }
    }


    /// <summary>
    /// Obtener las cuentas de usuarios de una organización.
    /// </summary>
    /// <param name="id">Id de la organización.</param>
    public async Task<ReadAllResponse<SessionModel<GroupMember>>> ReadUserAccounts(int id)
    {
        try
        {
            var members = await (from org in context.Organizations
                                 where org.Id == id
                                 join gm in context.GroupMembers
                                 on org.DirectoryId equals gm.GroupId
                                 join a in context.Accounts
                                 on gm.IdentityId equals a.IdentityId
                                 where a.Identity.Status == IdentityStatus.Enable
                                 select new SessionModel<GroupMember>
                                 {
                                     Account = new()
                                     {
                                         Id = a.Id,
                                         Name = a.Name,
                                         Visibility = a.Visibility,
                                         Identity = new()
                                         {
                                             Id = a.Identity.Id,
                                             Unique = a.Identity.Unique,
                                             Provider = a.Identity.Provider,
                                         }
                                     },
                                     Profile = gm
                                 }).ToListAsync();

            return new(Responses.Success, members);
        }
        catch (Exception)
        {
        }

        return new();

    }

}