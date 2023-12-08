namespace LIN.Identity.Data;


public class Directories
{


    #region Abstracciones


    /// <summary>
    /// Nuevo directorio vacío.
    /// </summary>
    /// <param name="directory">Modelo.</param>
    public static async Task<CreateResponse> Create(DirectoryModel directory)
    {

        // Obtiene la conexión
        var (context, connectionKey) = Conexión.GetOneConnection();

        var res = await Create(directory, context);
        context.CloseActions(connectionKey);
        return res;

    }



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
    /// Crear un directorio vacío.
    /// </summary>
    /// <param name="model">Modelo del directorio.</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<CreateResponse> Create(DirectoryModel model, Conexión context)
    {

        // Modelo.
        model.ID = 0;
        model.Members = [];

        // Ejecución
        try
        {

            // Agregar el elemento.
            await context.DataBase.Directories.AddAsync(model);

            // Guardar los cambios.
            context.DataBase.SaveChanges();

            // Respuesta.
            return new()
            {
                Response = Responses.Success,
                LastID = model.ID
            };

        }
        catch (Exception)
        {
        }

        return new();
    }



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