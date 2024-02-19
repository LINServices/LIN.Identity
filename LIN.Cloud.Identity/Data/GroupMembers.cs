namespace LIN.Cloud.Identity.Data;


public static class GroupMembers
{



    #region Abstracciones



    /// <summary>
    /// Crear nuevo integrante en un grupo.
    /// </summary>
    /// <param name="modelo">Modelo.</param>
    public static async Task<CreateResponse> Create(GroupMember modelo)
    {

        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnectionForce();

        // Función.
        var response = await Create(modelo, context);

        // Retornar.
        context.Close(contextKey);
        return response;

    }



    /// <summary>
    /// Crear nuevos integrantes en un grupo.
    /// </summary>
    /// <param name="modelos">Modelos.</param>
    public static async Task<CreateResponse> Create(IEnumerable<GroupMember> modelos)
    {

        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await Create(modelos, context);

        // Retornar.
        context.Close(contextKey);
        return response;

    }



    /// <summary>
    /// Obtener los miembros de un grupo.
    /// </summary>
    /// <param name="id">Id del grupo</param>
    public static async Task<ReadAllResponse<GroupMember>> ReadAll(int id)
    {

        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await ReadAll(id, context);

        // Retornar.
        context.Close(contextKey);
        return response;

    }



    /// <summary>
    /// Buscar en los integrantes de un grupo.
    /// </summary>
    /// <param name="pattern">Patron de búsqueda.</param>
    /// <param name="group">Id del grupo.</param>
    public static async Task<ReadAllResponse<IdentityModel>> Search(string pattern, int group)
    {

        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await Search(pattern, group, context);

        // Retornar.
        context.Close(contextKey);
        return response;

    }




    /// <summary>
    /// Buscar en los integrantes de un grupo.
    /// </summary>
    /// <param name="pattern">Patron de búsqueda.</param>
    /// <param name="group">Id del grupo.</param>
    public static async Task<ReadAllResponse<IdentityModel>> SearchGroups(string pattern, int group)
    {

        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await SearchGroups(pattern, group, context);

        // Retornar.
        context.Close(contextKey);
        return response;

    }



    /// <summary>
    /// Eliminar un integrante de un grupo.
    /// </summary>
    /// <param name="identity">Identidad.</param>
    /// <param name="group">Id del grupo.</param>
    public static async Task<ResponseBase> Delete(int identity, int group)
    {

        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await Delete(identity, group, context);

        // Retornar.
        context.Close(contextKey);
        return response;

    }




    /// <summary>
    /// Obtener los grupos donde una identidad esta de integrante
    /// </summary>
    /// <param name="id">Id del grupo</param>
    public static async Task<ReadAllResponse<GroupModel>> OnMembers(int organization, int identity)
    {

        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await OnMembers(organization, identity, context);

        // Retornar.
        context.Close(contextKey);
        return response;

    }

    #endregion



    /// <summary>
    /// Crear nuevo integrante en un grupo.
    /// </summary>
    /// <param name="modelo">Modelo.</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<CreateResponse> Create(GroupMember modelo, DataContext context)
    {
        try
        {

            // Ya existen.
            context.Attach(modelo.Group);
            context.Attach(modelo.Identity);

            // Guardar la identidad.
            await context.GroupMembers.AddAsync(modelo);
            context.SaveChanges();

            return new()
            {
                Response = Responses.Success
            };

        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.Undefined
            };
        }

    }



    /// <summary>
    /// Crear nuevos integrante en un grupo.
    /// </summary>
    /// <param name="modelos">Modelos.</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<CreateResponse> Create(IEnumerable<GroupMember> modelos, DataContext context)
    {
        try
        {

            // Validar existencia.
            foreach (var member in modelos)
            {
                try
                {
                    context.Database.ExecuteSqlRaw("""
                 INSERT INTO [dbo].[GROUPS_MEMBERS]
                      ([IdentityId]
                      ,[GroupId]
                      ,[Type])
                      VALUES
                      ({0}
                      ,{1}
                      ,{2}) 
                """, member.Identity.Id, member.Group.Id, (int)GroupMemberTypes.User);
                }

                catch (Exception ex)
                {

                }


            }

            return new()
            {
                Response = Responses.Success
            };

        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.Undefined
            };
        }

    }



    /// <summary>
    /// Obtener los miembros de un grupo.
    /// </summary>
    /// <param name="id">Id del grupo.</param>
    /// <param name="context">Contexto de base de datos.</param>
    public static async Task<ReadAllResponse<GroupMember>> ReadAll(int id, DataContext context)
    {

        try
        {

            // Consulta.
            var members = await (from gm in context.GroupMembers
                                 where gm.GroupId == id
                                 select new GroupMember
                                 {
                                     GroupId = gm.GroupId,
                                     Identity = gm.Identity,
                                     Type = gm.Type,
                                     IdentityId = gm.IdentityId
                                 }).ToListAsync();


            // Si la cuenta no existe.
            if (members == null)
                return new()
                {
                    Response = Responses.NotRows
                };

            // Success.
            return new()
            {
                Response = Responses.Success,
                Models = members
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
    /// Buscar en los integrantes de un grupo.
    /// </summary>
    /// <param name="pattern">Patron de búsqueda.</param>
    /// <param name="group">Id del grupo.</param>
    /// <param name="context">Contexto de base de datos.</param>
    public static async Task<ReadAllResponse<IdentityModel>> Search(string pattern, int group, DataContext context)
    {

        try
        {

            // Consulta.
            var members = await (from g in context.GroupMembers
                                 where g.GroupId == @group
                                 && g.Identity.Unique.ToLower().Contains(pattern.ToLower())
                                 select g.Identity).ToListAsync();


            // Si la cuenta no existe.
            if (members == null)
                return new()
                {
                    Response = Responses.NotRows
                };

            // Success.
            return new()
            {
                Response = Responses.Success,
                Models = members
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
    /// Buscar en los integrantes de un grupo.
    /// </summary>
    /// <param name="pattern">Patron de búsqueda.</param>
    /// <param name="group">Id del grupo.</param>
    /// <param name="context">Contexto de base de datos.</param>
    public static async Task<ReadAllResponse<IdentityModel>> SearchGroups(string pattern, int group, DataContext context)
    {

        try
        {

            // Consulta.
            var members = await (from g in context.GroupMembers
                                 where g.GroupId == @group
                                 && g.Identity.Unique.ToLower().Contains(pattern.ToLower())
                                 && g.Identity.Type == IdentityType.Group
                                 select g.Identity).ToListAsync();


            // Si la cuenta no existe.
            if (members == null)
                return new()
                {
                    Response = Responses.NotRows
                };

            // Success.
            return new()
            {
                Response = Responses.Success,
                Models = members
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
    /// Eliminar un integrante de un grupo.
    /// </summary>
    /// <param name="identity">Identidad.</param>
    /// <param name="group">Id del grupo.</param>
    /// <param name="context">Contexto de base de datos.</param>
    public static async Task<ResponseBase> Delete(int identity, int group, DataContext context)
    {

        try
        {


            var response = await (from g in context.GroupMembers
                                  where g.GroupId == @group
                                  && g.IdentityId == identity
                                  select g).ExecuteDeleteAsync();


            // Success.
            return new()
            {
                Response = Responses.Success
            };

        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.Undefined
            };
        }

    }



    /// <summary>
    /// Obtener los grupos donde una identidad esta de integrante
    /// </summary>
    /// <param name="organization"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public static async Task<ReadAllResponse<GroupModel>> OnMembers(int organization, int identity, DataContext context)
    {

        try
        {

            // Consulta.
            var groups = await (from g in context.GroupMembers
                                where g.Group.OwnerId == organization
                                && g.IdentityId == identity
                                select new GroupModel
                                {
                                    Id = g.Group.Id,
                                    Identity = g.Group.Identity,
                                    Name = g.Group.Name,
                                    OwnerId = g.Group.OwnerId
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



}