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
    /// Obtiene los integrantes de un directorio.
    /// </summary>
    /// <param name="identityId">Id del directorio.</param>
    public static async Task<ReadAllResponse<DirectoryMember>> ReadMembers(int identityId)
    {

        // Obtiene la conexión
        var (context, connectionKey) = Conexión.GetOneConnection();

        var res = await ReadMembers(identityId, context);
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
    /// Obtiene los integrantes de un directorio.
    /// </summary>
    /// <param name="directoryId">Id del directorio.</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<ReadAllResponse<DirectoryMember>> ReadMembers(int directoryId, Conexión context)
    {

        // Ejecución
        try
        {

            // Directorios.
            var directories = await (from directoryMember in context.DataBase.DirectoryMembers
                                     where directoryMember.DirectoryId == directoryId
                                     select new DirectoryMember
                                     {
                                         Rol = directoryMember.Rol,
                                         DirectoryId = directoryMember.DirectoryId,
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