namespace LIN.Identity.Data;


public class Directories
{


    #region Abstracciones



    /// <summary>
    /// Obtener un directorio.
    /// </summary>
    /// <param name="id">Id del directorio.</param>
    public static async Task<ReadOneResponse<DirectoryModel>> Read(int id)
    {

        // Obtiene la conexión
        var (context, connectionKey) = Conexión.GetOneConnection();

        var res = await Read(id, context);
        context.CloseActions(connectionKey);
        return res;

    }



    #endregion



    /// <summary>
    /// Obtener un directorio.
    /// </summary>
    /// <param name="id">Id del directorio.</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<ReadOneResponse<DirectoryModel>> Read(int id, Conexión context)
    {

        // Ejecución
        try
        {

            // Consulta.
            var query = Queries.Accounts.GetDirectory(id, context);

            // Obtiene el usuario
            var result = await query.FirstOrDefaultAsync();

            // Si no existe el modelo
            if (result == null)
                return new(Responses.NotExistAccount);

            return new(Responses.Success, result);
        }
        catch (Exception)
        {
        }

        return new();
    }



}