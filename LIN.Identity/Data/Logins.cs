namespace LIN.Auth.Data;

public class Logins
{



    #region Abstracciones


    /// <summary>
    /// Crea un registro de Login
    /// </summary>
    /// <param name="data">Modelo del login</param>
    public async static Task<CreateResponse> Create(LoginLogModel data)
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
    public async static Task<ReadAllResponse<LoginLogModel>> ReadAll(int id)
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
    public async static Task<CreateResponse> Create(LoginLogModel data, Conexión context)
    {
        // ID en 0
        data.ID = 0;

        // Ejecución
        try
        {

            var appResult = await Data.Applications.Read(data.Application.Key, context);


            if (appResult.Response != Responses.Success)
            {
                return new(Responses.NotRows);
            }


            data.Application = new()
            {
                ID = appResult.Model.ID
            };

            try
            {
                context.DataBase.Attach(data.Application);
            }
            catch
            {
            }


            var res = context.DataBase.LoginLogs.Add(data);
            await context.DataBase.SaveChangesAsync();
            return new(Responses.Success, data.ID);
        }
        catch
        {
        }

        return new();
    }



    /// <summary>
    /// Obtiene la lista de registros de acceso de una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadAllResponse<LoginLogModel>> ReadAll(int id, Conexión context)
    {

        // Ejecución
        try
        {

            var logins = from L in context.DataBase.LoginLogs
                         where L.AccountID == id
                         orderby L.Date descending
                         select new LoginLogModel
                         {
                             ID = L.ID,
                             Type = L.Type,
                             Date = L.Date,
                             Platform = L.Platform,
                             Application = new()
                             {
                                 Name = L.Application.Name,
                                 Badge = L.Application.Badge
                             }
                         };


            var result = await logins.Take(50).ToListAsync();

            return new(Responses.Success, result);
        }
        catch
        {
        }
        return new();
    }



}