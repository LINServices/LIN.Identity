namespace LIN.Identity.Data;


public class Directories
{


    public static async Task<ReadOneResponse<DirectoryModel>> Read(int id)
    {

        // Obtiene la conexión
        var (context, connectionKey) = Conexión.GetOneConnection();

        var res = await Read(id, context);
        context.CloseActions(connectionKey);
        return res;

    }




    public static async Task<ReadOneResponse<DirectoryModel>> Read(int id, Conexión context)
    {

        // Ejecución
        try
        {

            var query = Queries.Accounts.GetDirectory(id, context);

            // Obtiene el usuario
            var result = await query.FirstOrDefaultAsync();

            // Si no existe el modelo
            if (result == null)
                return new(Responses.NotExistAccount);

            return new(Responses.Success, result);
        }
        catch
        {
        }

        return new();
    }



}