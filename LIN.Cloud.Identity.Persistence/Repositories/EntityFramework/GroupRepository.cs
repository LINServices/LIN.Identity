namespace LIN.Cloud.Identity.Persistence.Repositories.EntityFramework;

internal class GroupRepository(DataContext context) : IGroupRepository
{

    /// <summary>
    /// Crear nuevo grupo.
    /// </summary>
    /// <param name="modelo">Modelo.</param>
    public async Task<ReadOneResponse<GroupModel>> Create(GroupModel modelo)
    {
        modelo.Id = 0;

        using var transaction = context.Database.BeginTransaction();

        try
        {
            foreach (var e in modelo.Members)
            {
                e.Group = modelo;
                e.Identity = context.AttachOrUpdate(e.Identity)!;
            }

            modelo.Identity.Owner = new()
            {
                Id = modelo.Identity.OwnerId ?? 0
            };

            modelo.Identity.Owner = context.AttachOrUpdate(modelo.Identity.Owner);

            await context.Groups.AddAsync(modelo);

            var generalGroupInformation = (from org in context.Organizations
                                           where org.Id == modelo.Identity.OwnerId
                                           select new { org.DirectoryId, org.Directory.Identity.Unique }).FirstOrDefault();

            if (generalGroupInformation is null)
            {
                transaction.Rollback();
                return new(Responses.NotRows);
            }

            // El unique del grupo se compone anidando el de su directorio general para evitar colisiones entre organizaciones.
            modelo.Identity.Unique = $"{modelo.Identity.Unique}@{generalGroupInformation.Unique}";

            context.SaveChanges();

            var generalDirectory = new GroupModel
            {
                Id = generalGroupInformation.DirectoryId
            };

            generalDirectory = context.AttachOrUpdate(generalDirectory);

            context.GroupMembers.Add(new()
            {
                Group = generalDirectory!,
                Identity = modelo.Identity,
                Type = GroupMemberTypes.Group
            });

            context.SaveChanges();
            transaction.Commit();

            return new(Responses.Success, modelo);
        }
        catch (Exception)
        {
            transaction.Rollback();
            return new();
        }
    }


    /// <summary>
    /// Obtener un grupo según el Id.
    /// </summary>
    /// <param name="id">Id.</param>
    public async Task<ReadOneResponse<GroupModel>> Read(int id)
    {
        try
        {
            var group = await (from g in context.Groups
                               where g.Id == id
                               select new GroupModel
                               {
                                   Id = g.Id,
                                   Identity = g.Identity,
                                   Name = g.Name,
                                   IdentityId = g.IdentityId,
                                   Description = g.Description
                               }).FirstOrDefaultAsync();

            if (group is null)
                return new(Responses.NotRows);

            return new(Responses.Success, group);
        }
        catch (Exception)
        {
            return new();
        }

    }


    /// <summary>
    /// Obtener un grupo según el Id de la identidad.
    /// </summary>
    /// <param name="id">Identidad.</param>
    public async Task<ReadOneResponse<GroupModel>> ReadByIdentity(int id)
    {
        try
        {
            var group = await (from g in context.Groups
                               where g.IdentityId == id
                               select new GroupModel
                               {
                                   Id = g.Id,
                                   Identity = g.Identity,
                                   Name = g.Name,
                                   IdentityId = g.IdentityId,
                                   Description = g.Description
                               }).FirstOrDefaultAsync();

            if (group is null)
                return new(Responses.NotRows);

            return new(Responses.Success, group);
        }
        catch (Exception)
        {
            return new();
        }

    }


    /// <summary>
    /// Obtener los grupos asociados a una organización.
    /// </summary>
    /// <param name="organization">Organización.</param>
    public async Task<ReadAllResponse<GroupModel>> ReadAll(int organization)
    {
        try
        {
            var groups = await (from g in context.Groups
                                where g.Identity.OwnerId == organization
                                select new GroupModel
                                {
                                    Id = g.Id,
                                    Identity = g.Identity,
                                    Name = g.Name
                                }).ToListAsync();

            return new(Responses.Success, groups ?? []);
        }
        catch (Exception)
        {
            return new();
        }
    }


    /// <summary>
    /// Obtener la organización propietaria de un grupo.
    /// </summary>
    /// <param name="id">Id del grupo.</param>
    public async Task<ReadOneResponse<int>> GetOwner(int id)
    {
        try
        {

            var ownerId = await (from g in context.Groups
                                 where g.Id == id
                                 select g.Identity.OwnerId).FirstOrDefaultAsync();

            if (ownerId is null || ownerId.Value <= 0)
                return new(Responses.NotRows);

            return new(Responses.Success, ownerId ?? 0);
        }
        catch (Exception)
        {
            return new();
        }

    }


    /// <summary>
    /// Obtener la organización propietaria de un grupo.
    /// </summary>
    /// <param name="id">Id de la identidad.</param>
    public async Task<ReadOneResponse<int>> GetOwnerByIdentity(int id)
    {
        try
        {

            var ownerId = await (from g in context.Groups
                                 where g.IdentityId == id
                                 select g.Identity.OwnerId).FirstOrDefaultAsync();

            if (ownerId is null || ownerId.Value <= 0)
                return new(Responses.NotRows);

            return new(Responses.Success, ownerId ?? 0);
        }
        catch (Exception)
        {
            return new();
        }

    }

}