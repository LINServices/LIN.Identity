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

        // Transacción.
        using var transaction = context.Database.BeginTransaction();

        try
        {
            // Miembros.
            foreach (var e in modelo.Members)
            {
                e.Group = modelo;
                e.Identity = context.AttachOrUpdate(e.Identity)!;
            }

            // Fijar la organización.
            modelo.Identity.Owner = new()
            {
                Id = modelo.Identity.OwnerId ?? 0
            };

            modelo.Identity.Owner = context.AttachOrUpdate(modelo.Identity.Owner);

            // Guardar la identidad.
            await context.Groups.AddAsync(modelo);

            // Obtener el directorio general.
            var generalGroupInformation = (from org in context.Organizations
                                           where org.Id == modelo.Identity.OwnerId
                                           select new { org.DirectoryId, org.Directory.Identity.Unique }).FirstOrDefault();

            // Si no se encontró el directorio.
            if (generalGroupInformation is null)
            {
                transaction.Rollback();
                return new(Responses.NotRows);
            }

            // Nueva identidad.
            modelo.Identity.Unique = $"{modelo.Identity.Unique}@{generalGroupInformation.Unique}";

            // Guardar.
            context.SaveChanges();

            // Agregar el grupo al directorio general.
            var generalDirectory = new GroupModel
            {
                Id = generalGroupInformation.DirectoryId
            };

            // Si no se encontró el directorio.
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
            // Consulta.
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

            // Si la cuenta no existe.
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
            // Consulta.
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

            // Si la cuenta no existe.
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
            // Consulta.
            var groups = await (from g in context.Groups
                                where g.Identity.OwnerId == organization
                                select new GroupModel
                                {
                                    Id = g.Id,
                                    Identity = g.Identity,
                                    Name = g.Name
                                }).ToListAsync();

            // Success.
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

            // Consulta.
            var ownerId = await (from g in context.Groups
                                 where g.Id == id
                                 select g.Identity.OwnerId).FirstOrDefaultAsync();

            // Si la cuenta no existe.
            if (ownerId is null || ownerId.Value <= 0)
                return new(Responses.NotRows);

            // Success.
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

            // Consulta.
            var ownerId = await (from g in context.Groups
                                 where g.IdentityId == id
                                 select g.Identity.OwnerId).FirstOrDefaultAsync();

            // Si la cuenta no existe.
            if (ownerId is null || ownerId.Value <= 0)
                return new(Responses.NotRows);

            // Success.
            return new(Responses.Success, ownerId ?? 0);
        }
        catch (Exception)
        {
            return new();
        }

    }

}