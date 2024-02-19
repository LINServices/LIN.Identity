namespace LIN.Cloud.Identity.Data;


public static class Groups
{



    #region Abstracciones



    /// <summary>
    /// Crear un nuevo grupo.
    /// </summary>
    /// <param name="modelo">Modelo.</param>
    public static async Task<ReadOneResponse<GroupModel>> Create(GroupModel modelo)
    {

        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await Create(modelo, context);

        // Retornar.
        context.Close(contextKey);
        return response;

    }



    /// <summary>
    /// Obtener un grupo según el Id.
    /// </summary>
    /// <param name="id">Id.</param>
    public static async Task<ReadOneResponse<GroupModel>> Read(int id)
    {

        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await Read(id, context);

        // Retornar.
        context.Close(contextKey);
        return response;

    }




    /// <summary>
    /// Obtener un grupo según el Id de la identidad.
    /// </summary>
    /// <param name="id">Id.</param>
    public static async Task<ReadOneResponse<GroupModel>> ReadByIdentity(int id)
    {

        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await ReadByIdentity(id, context);

        // Retornar.
        context.Close(contextKey);
        return response;

    }



    /// <summary>
    /// Obtener los grupos asociados a una organización.
    /// </summary>
    /// <param name="organization">Id de la organización.</param>
    public static async Task<ReadAllResponse<GroupModel>> ReadAll(int organization)
    {

        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await ReadAll(organization, context);

        // Retornar.
        context.Close(contextKey);
        return response;

    }



    /// <summary>
    /// Obtener la organización propietaria de un grupo.
    /// </summary>
    /// <param name="group">Id del grupo.</param>
    public static async Task<ReadOneResponse<int>> GetOwner(int group)
    {

        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await GetOwner(group, context);

        // Retornar.
        context.Close(contextKey);
        return response;

    }



    /// <summary>
    /// Obtener la organización propietaria de un grupo.
    /// </summary>
    /// <param name="identity">Id de la identidad del grupo.</param>
    public static async Task<ReadOneResponse<int>> GetOwnerByIdentity(int identity)
    {

        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await GetOwnerByIdentity(identity, context);

        // Retornar.
        context.Close(contextKey);
        return response;

    }



    #endregion



    /// <summary>
    /// Crear nuevo grupo.
    /// </summary>
    /// <param name="modelo">Modelo.</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<ReadOneResponse<GroupModel>> Create(GroupModel modelo, DataContext context)
    {
        // Pre.
        modelo.Id = 0;

        // Transacción.
        using var transaction = context.Database.BeginTransaction();

        try
        {

            // Miembros.
            foreach (var e in modelo.Members)
            {
                e.Group = modelo;
                context.Attach(e.Identity);
            }

            // Fijar la organización.
            modelo.Owner = new()
            {
                Id = modelo.OwnerId ?? 0
            };

            // Si no existe.
            if (modelo.Owner != null)
                context.Attach(modelo.Owner);

            // Guardar la identidad.
            await context.Groups.AddAsync(modelo);


            // Obtener el directorio general.
            var dir = (from org in context.Organizations
                       where org.Id == modelo.OwnerId
                       select new { org.DirectoryId, org.Directory.Identity.Unique }).FirstOrDefault();

            // Si no se encontró el directorio.
            if (dir == null)
            {
                transaction.Rollback();
                return new()
                {
                    Response = Responses.NotRows
                };
            }

            // Nueva identidad.
            modelo.Identity.Unique = $"{modelo.Identity.Unique}@{dir.Unique}";

            // Guardar.
            context.SaveChanges();


            var dirModel = new GroupModel
            {
                Id = dir.DirectoryId
            };

            context.Attach(dirModel);

            context.GroupMembers.Add(new()
            {
                Group = dirModel,
                Identity = modelo.Identity,
                Type = GroupMemberTypes.Group
            });


            context.SaveChanges();

            transaction.Commit();

            return new()
            {
                Response = Responses.Success,
                Model = modelo
            };

        }
        catch (Exception)
        {
            transaction.Rollback();
            return new()
            {
                Response = Responses.Undefined
            };
        }

    }



    /// <summary>
    /// Obtener un grupo según el Id.
    /// </summary>
    /// <param name="id">Id.</param>
    /// <param name="context">Contexto de base de datos.</param>
    public static async Task<ReadOneResponse<GroupModel>> Read(int id, DataContext context)
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
            if (group == null)
                return new()
                {
                    Response = Responses.NotRows
                };

            // Success.
            return new()
            {
                Response = Responses.Success,
                Model = group
            };

        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.ExistAccount
            };
        }

    }



    /// <summary>
    /// Obtener un grupo según el Id de la identidad.
    /// </summary>
    /// <param name="id">Identidad.</param>
    /// <param name="context">Contexto de base de datos.</param>
    public static async Task<ReadOneResponse<GroupModel>> ReadByIdentity(int id, DataContext context)
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
            if (group == null)
                return new()
                {
                    Response = Responses.NotRows
                };

            // Success.
            return new()
            {
                Response = Responses.Success,
                Model = group
            };

        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.ExistAccount
            };
        }

    }



    /// <summary>
    /// Obtener los grupos asociados a una organización.
    /// </summary>
    /// <param name="organization">Organización.</param>
    /// <param name="context">Contexto de base de datos.</param>
    public static async Task<ReadAllResponse<GroupModel>> ReadAll(int organization, DataContext context)
    {

        try
        {

            // Consulta.
            var groups = await (from g in context.Groups
                                where g.OwnerId == organization
                                select new GroupModel
                                {
                                    Id = g.Id,
                                    Identity = g.Identity,
                                    Name = g.Name,
                                    OwnerId = g.OwnerId
                                }).ToListAsync();

            // Si la cuenta no existe.
            if (groups == null)
                return new()
                {
                    Response = Responses.NotRows
                };

            // Success.
            return new()
            {
                Response = Responses.Success,
                Models = groups
            };

        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.ExistAccount
            };
        }

    }



    /// <summary>
    /// Obtener la organización propietaria de un grupo.
    /// </summary>
    /// <param name="id">Id del grupo.</param>
    /// <param name="context">Contexto de base de datos.</param>
    public static async Task<ReadOneResponse<int>> GetOwner(int id, DataContext context)
    {

        try
        {

            // Consulta.
            var ownerId = await (from g in context.Groups
                                 where g.Id == id
                                 select g.OwnerId).FirstOrDefaultAsync();

            // Si la cuenta no existe.
            if (ownerId == null)
                return new()
                {
                    Response = Responses.NotRows
                };

            // Success.
            return new()
            {
                Response = ownerId == null ? Responses.NotRows : Responses.Success,
                Model = ownerId ?? 0
            };

        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.ExistAccount
            };
        }

    }



    /// <summary>
    /// Obtener la organización propietaria de un grupo.
    /// </summary>
    /// <param name="id">Id de la identidad.</param>
    /// <param name="context">Contexto de base de datos.</param>
    public static async Task<ReadOneResponse<int>> GetOwnerByIdentity(int id, DataContext context)
    {

        try
        {

            // Consulta.
            var ownerId = await (from g in context.Groups
                                 where g.IdentityId == id
                                 select g.OwnerId).FirstOrDefaultAsync();

            // Si la cuenta no existe.
            if (ownerId == null)
                return new()
                {
                    Response = Responses.NotRows
                };

            // Success.
            return new()
            {
                Response = ownerId == null ? Responses.NotRows : Responses.Success,
                Model = ownerId ?? 0
            };

        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.ExistAccount
            };
        }

    }



}