namespace LIN.Auth.Data;

public class Logins
{



    #region Abstracciones


    /// <summary>
    /// Crea un registro de Login
    /// </summary>
    /// <param name="data">Modelo del login</param>
    public async static Task<CreateResponse> Create(UserAccessLogDataModel data)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();
        var res = await Create(data, context);
        context.CloseActions(connectionKey);
        return res;
    }



    /// <summary>
    /// Obtiene la lista de registros login de una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    public async static Task<ReadAllResponse<UserAccessLogDataModel>> ReadAll(int id)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await ReadAll(id, context);
        context.CloseActions(connectionKey);
        return res;

    }


    #endregion



    /// <summary>
    /// Crea un registro de Login
    /// </summary>
    /// <param name="data">Modelo del login</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<CreateResponse> Create(UserAccessLogDataModel data, Conexión context)
    {
        // ID en 0
        data.ID = 0;

        // Ejecución
        try
        {
            var res = context.DataBase.Logins.Add(data);
            await context.DataBase.SaveChangesAsync();
            return new(Responses.Success, data.ID);
        }
        catch (Exception ex)
        {
            context.DataBase.Remove(data);
            ServerLogger.LogError(ex.Message);
        }

        return new();
    }



    /// <summary>
    /// Obtiene la lista de registros de acceso de una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadAllResponse<UserAccessLogDataModel>> ReadAll(int id, Conexión context)
    {

        // Ejecución
        try
        {
            List<UserAccessLogDataModel> res = await context.DataBase.Logins.Where(T => T.UserID == id).OrderByDescending(T => T.Date).Take(10).ToListAsync();

            var lista = res;

            return new(Responses.Success, lista);
        }
        catch (Exception ex)
        {
            ServerLogger.LogError(ex.Message);
        }
        return new();
    }



}