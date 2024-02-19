using LIN.Types.Cloud.Identity.Abstracts;

namespace LIN.Cloud.Identity.Data;


public static class DirectoryMembers
{



    #region Abstracciones



    /// <summary>
    /// Obtener los directorios (Grupos de organización) donde una identidad pertenece.
    /// </summary>
    /// <param name="id">Id de la identidad</param>
    public static async Task<ReadAllResponse<GroupMember>> Read(int id, int organization)
    {

        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await Read(id, organization, context);

        // Retornar.
        context.Close(contextKey);
        return response;

    }



    /// <summary>
    /// Valida si una identidad es miembro de una organización.
    /// </summary>
    /// <param name="id">Identidad</param>
    /// <param name="organization">Id de la organización</param>
    public static async Task<ReadOneResponse<GroupMemberTypes>> IamIn(int id, int organization)
    {

        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await IamIn(id, organization, context);

        // Retornar.
        context.Close(contextKey);
        return response;

    }







    public static async Task<ReadAllResponse<int>> IamIn(List<int> id, int organization)
    {

        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await IamIn(id, organization, context);

        // Retornar.
        context.Close(contextKey);
        return response;

    }



    /// <summary>
    /// Valida si una identidad es miembro de una organización.
    /// </summary>
    /// <param name="id">Identidad</param>
    /// <param name="directory">Id de la organización</param>
    public static async Task<ReadOneResponse<GroupMemberTypes>> IamInByDir(int id, int directory)
    {

        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await IamInByDir(id, directory, context);

        // Retornar.
        context.Close(contextKey);
        return response;

    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">Id del directorio</param>
    public static async Task<ReadAllResponse<GroupMember>> ReadMembers(int id)
    {

        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await ReadMembers(id, context);

        // Retornar.
        context.Close(contextKey);
        return response;

    }



    public static async Task<ReadAllResponse<SessionModel<GroupMember>>> ReadMembersByOrg(int id)
    {

        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await ReadMembersByOrg(id, context);

        // Retornar.
        context.Close(contextKey);
        return response;

    }




    #endregion






    /// <summary>
    /// Obtener los directorios (Grupos de organización) donde una identidad pertenece.
    /// </summary>
    /// <param name="id">Identidad</param>
    /// <param name="organization">Organización de contexto.</param>
    /// <param name="context">Contexto.</param>
    public static async Task<ReadAllResponse<GroupMember>> Read(int id, int organization, DataContext context)
    {

        try
        {


            var members = await (from gm in context.GroupMembers
                                 where gm.IdentityId == id
                                 join o in context.Organizations
                                 on gm.GroupId equals o.DirectoryId
                                 where o.Id == organization
                                 select new GroupMember
                                 {
                                     Type = gm.Type,
                                     Group = gm.Group,
                                     GroupId = gm.GroupId,
                                     IdentityId = gm.IdentityId,
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
    /// Valida si una identidad es miembro de una organización.
    /// </summary>
    /// <param name="id">Identidad</param>
    /// <param name="organization">Id de la organización</param>
    /// <param name="context">Contexto</param>
    public static async Task<ReadOneResponse<GroupMemberTypes>> IamIn(int id, int organization, DataContext context)
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
            if (query == null)
                return new()
                {
                    Response = Responses.NotRows
                };

            // Success.
            return new()
            {
                Response = Responses.Success,
                Model = query.Type
            };

        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.NotRows
            };
        }

    }




    /// <summary>
    /// Valida si una lista de identidades son miembro de una organización.
    /// </summary>
    /// <param name="ids">Identidades</param>
    /// <param name="organization">Id de la organización</param>
    /// <param name="context">Contexto</param>
    public static async Task<ReadAllResponse<int>> IamIn(List<int> ids, int organization, DataContext context)
    {

        try
        {

            // Consulta.
            var query = await (from org in context.Organizations
                               where org.Id == organization
                               join gm in context.GroupMembers
                               on org.DirectoryId equals gm.GroupId
                               where ids.Contains(gm.IdentityId)
                               select gm.IdentityId).ToListAsync();


            // Si la cuenta no existe.
            if (query == null)
                return new()
                {
                    Response = Responses.NotRows
                };

            // Success.
            return new()
            {
                Response = Responses.Success,
                Models = query
            };

        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.NotRows
            };
        }

    }



    /// <summary>
    /// Valida si una identidad es miembro de una organización.
    /// </summary>
    /// <param name="id">Identidad</param>
    /// <param name="directory">Id del directorio</param>
    /// <param name="context">Contexto</param>
    public static async Task<ReadOneResponse<GroupMemberTypes>> IamInByDir(int id, int directory, DataContext context)
    {

        try
        {

            // Consulta.
            var query = await (from org in context.Organizations
                               where org.DirectoryId == directory
                               join gm in context.GroupMembers
                               on org.DirectoryId equals gm.GroupId
                               where gm.IdentityId == id
                               select new
                               {
                                   gm.Type
                               }).FirstOrDefaultAsync();


            // Si la cuenta no existe.
            if (query == null)
                return new()
                {
                    Response = Responses.NotRows
                };

            // Success.
            return new()
            {
                Response = Responses.Success,
                Model = query.Type
            };

        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.NotRows
            };
        }

    }



    /// <summary>
    ///
    /// </summary>
    /// <param name="id">Directorio</param>
    /// <param name="context">Contexto</param>
    public static async Task<ReadAllResponse<GroupMember>> ReadMembers(int id, DataContext context)
    {

        try
        {

            var members = await (from org in context.Organizations
                                 where org.DirectoryId == id
                                 join gm in context.GroupMembers
                                 on org.DirectoryId equals gm.GroupId
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
    ///
    /// </summary>
    /// <param name="id">Directorio</param>
    /// <param name="context">Contexto</param>
    public static async Task<ReadAllResponse<SessionModel<GroupMember>>> ReadMembersByOrg(int id, DataContext context)
    {

        try
        {

            var members = await (from org in context.Organizations
                                 where org.Id == id
                                 join gm in context.GroupMembers
                                 on org.DirectoryId equals gm.GroupId
                                 join a in context.Accounts
                                 on gm.IdentityId equals a.IdentityId
                                 select new SessionModel<GroupMember>
                                 {
                                      Account = new()
                                      {
                                          Id = a.Id,
                                          Name = a.Name,
                                          Visibility = a.Visibility,
                                          IdentityService = a.IdentityService,
                                          Identity = new()
                                          {
                                              Id = a.Identity.Id,
                                              Unique = a.Identity.Unique
                                          }
                                      },
                                      Profile = gm
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




}