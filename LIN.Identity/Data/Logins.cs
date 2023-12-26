namespace LIN.Identity.Data;


public class Logins
{



    #region Abstracciones


    /// <summary>
    /// Crea un registro de Login
    /// </summary>
    /// <param name="data">Modelo del login</param>
    public static async Task<CreateResponse> Create(LoginLogModel data)
    {

        // Obtiene la conexión
        var (context, connectionKey) = Conexión.GetOneConnection();
        var res = await Create(data, context);
        context.CloseActions(connectionKey);
        return res;
    }



    /// <summary>
    /// Obtiene la lista de registros login de una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    public static async Task<ReadAllResponse<LoginLogModel>> ReadAll(int id)
    {

        // Obtiene la conexión
        var (context, connectionKey) = Conexión.GetOneConnection();

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
    public static async Task<CreateResponse> Create(LoginLogModel data, Conexión context)
    {
        // ID en 0
        data.ID = 0;

        // Ejecución
        try
        {

            // Ya existe la app.
            context.DataBase.Attach(data.Application);

            var res = context.DataBase.LoginLogs.Add(data);
            await context.DataBase.SaveChangesAsync();
            return new(Responses.Success, data.ID);
        }
        catch (Exception)
        {
        }

        return new();
    }



    /// <summary>
    /// Obtiene la lista de registros de acceso de una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="context">Contexto de conexión</param>
    public static async Task<ReadAllResponse<LoginLogModel>> ReadAll(int id, Conexión context)
    {

        // Ejecución
        try
        {

            // Consulta.
            var logins = from L in context.DataBase.LoginLogs
                         where L.AccountID == id
                         orderby L.Date descending
                         select new LoginLogModel
                         {
                             ID = L.ID,
                             Type = L.Type,
                             Date = L.Date,
                             Application = new()
                             {
                                 ID = L.Application.ID,
                                 Name = L.Application.Name,
                                 Badge = L.Application.Badge
                             }
                         };

            // Resultado.
            var result = await logins.Take(50).ToListAsync();

            return new(Responses.Success, result);
        }
        catch (Exception)
        {
        }
        return new();
    }



}