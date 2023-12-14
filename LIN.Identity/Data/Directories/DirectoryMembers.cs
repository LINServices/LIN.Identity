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

            var (_, identities) = await Queries.Directories.Get(identityId);

            // Directorios.
            var directories = await (from directoryMember in context.DataBase.DirectoryMembers
                                     where identities.Contains(directoryMember.IdentityId)
                                     select new DirectoryMember
                                     {
                                         Rol = directoryMember.Rol,
                                         DirectoryId = directoryMember.DirectoryId,
                                         Directory = new()
                                         {
                                             ID = directoryMember.Directory.ID,
                                             Creación = directoryMember.Directory.Creación,
                                             Nombre = directoryMember.Directory.Nombre,
                                             IdentityId = directoryMember.Directory.IdentityId,
                                             Identity = new()
                                             {
                                                 Id = directoryMember.Directory.Identity.Id,
                                                 Unique = directoryMember.Directory.Identity.Unique,
                                                 Type = directoryMember.Directory.Identity.Type
                                             }
                                         },
                                         Identity = new()
                                         {
                                             Id = directoryMember.Identity.Id,
                                             Unique = directoryMember.Identity.Unique,
                                             Type = directoryMember.Identity.Type
                                         },
                                         IdentityId = directoryMember.IdentityId
                                     }).ToListAsync();

            return new(Responses.Success, directories);

        }
        catch (Exception)
        {
        }

        return new();
    }


}