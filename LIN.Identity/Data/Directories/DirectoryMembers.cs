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
            context.DataBase.Attach(model.Account);
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


}