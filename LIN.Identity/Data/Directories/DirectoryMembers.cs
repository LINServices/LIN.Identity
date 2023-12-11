namespace LIN.Identity.Data;


public class DirectoryMembers
{


    #region Abstracciones


    /// <summary>
    /// Agregar un miembro a un directorio.
    /// </summary>
    /// <param name="directory">Modelo.</param>
    public static async Task<CreateResponse> Create(DirectoryMember directory)
    {

        // Obtiene la conexión
        var (context, connectionKey) = Conexión.GetOneConnection();

        var res = await Create(directory, context);
        context.CloseActions(connectionKey);
        return res;

    }



    /// <summary>
    /// Obtiene los directorios donde un usuario es integrante.
    /// </summary>
    /// <param name="accountId">Id de la cuenta.</param>
    public static async Task<ReadAllResponse<DirectoryMember>> ReadAll(int accountId)
    {

        // Obtiene la conexión
        var (context, connectionKey) = Conexión.GetOneConnection();

        var res = await ReadAll(accountId, context);
        context.CloseActions(connectionKey);
        return res;

    }



    #endregion



    /// <summary>
    /// Agregar un miembro a un directorio.
    /// </summary>
    /// <param name="model">Modelo.</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<CreateResponse> Create(DirectoryMember model, Conexión context)
    {

        // Ejecución
        try
        {

            // Ya existen los registros.
            context.DataBase.Attach(model.Identity);
            context.DataBase.Attach(model.Directory);

            // Agregar el elemento.
            await context.DataBase.DirectoryMembers.AddAsync(model);

            // Guardar los cambios.
            context.DataBase.SaveChanges();

            // Respuesta.
            return new()
            {
                Response = Responses.Success
            };

        }
        catch (Exception)
        {
        }

        return new();
    }



    /// <summary>
    /// Obtiene los directorios donde un usuario es integrante.
    /// </summary>
    /// <param name="accountId">Id de la cuenta.</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<ReadAllResponse<DirectoryMember>> ReadAll(int identityId, Conexión context)
    {

        // Ejecución
        try
        {

            // Directorios.
            var directories = await (from directory in context.DataBase.DirectoryMembers
                                     where directory.IdentityId == identityId
                                     select new DirectoryMember
                                     {
                                         Rol = directory.Rol,
                                         DirectoryId = directory.DirectoryId,
                                         Directory = new()
                                         {
                                             ID = directory.Directory.ID,
                                             Creación = directory.Directory.Creación,
                                             Nombre = directory.Directory.Nombre,
                                             IdentityId = directory.Directory.IdentityId,
                                             Identity = new()
                                             {
                                                 Id = directory.Directory.Identity.Id,
                                                 Unique = directory.Directory.Identity.Unique,
                                                 Type = directory.Directory.Identity.Type
                                             }
                                         },
                                         IdentityId = directory.IdentityId
                                     }).ToListAsync();

            return new(Responses.Success, directories);

        }
        catch (Exception)
        {
        }

        return new();
    }


}