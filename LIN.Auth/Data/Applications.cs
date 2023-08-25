namespace LIN.Auth.Data;


public class Applications
{


    #region Abstracciones


    /// <summary>
    /// Crea una app
    /// </summary>
    /// <param name="data">Modelo</param>
    public async static Task<CreateResponse> Create(ApplicationModel data)
    {
        var (context, contextKey) = Conexión.GetOneConnection();
        var response = await Create(data, context);
        context.CloseActions(contextKey);
        return response;
    }



    /// <summary>
    /// Obtiene la lista de apps asociados a una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    public async static Task<ReadAllResponse<ApplicationModel>> ReadAll(int id)
    {
        var (context, contextKey) = Conexión.GetOneConnection();

        var res = await ReadAll(id, context);
        context.CloseActions(contextKey);
        return res;
    }



    /// <summary>
    /// Obtiene una app
    /// </summary>
    /// <param name="id">ID de la app</param>
    public async static Task<ReadOneResponse<ApplicationModel>> Read(int id)
    {
        var (context, contextKey) = Conexión.GetOneConnection();

        var res = await Read(id, context);
        context.CloseActions(contextKey);
        return res;
    }


    /// <summary>
    /// Obtiene una app
    /// </summary>
    /// <param name="key">Key de la app</param>
    public async static Task<ReadOneResponse<ApplicationModel>> Read(string key)
    {
        var (context, contextKey) = Conexión.GetOneConnection();

        var res = await Read(key, context);
        context.CloseActions(contextKey);
        return res;
    }


    #endregion



    /// <summary>
    /// Crear aplicación
    /// </summary>
    /// <param name="data">Modelo</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<CreateResponse> Create(ApplicationModel data, Conexión context)
    {

        data.ID = 0;

        // Ejecución
        try
        {

            var res = await context.DataBase.Applications.AddAsync(data);
            context.DataBase.SaveChanges();

            return new(Responses.Success, data.ID);
        }
        catch (Exception ex)
        {

            if ((ex.InnerException?.Message.Contains("Violation of UNIQUE KEY constraint") ?? false) || (ex.InnerException?.Message.Contains("duplicate key") ?? false))
                return new(Responses.Undefined);

        }

        return new();
    }



    /// <summary>
    /// Obtiene la lista de apps asociados a una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadAllResponse<ApplicationModel>> ReadAll(int id, Conexión context)
    {

        // Ejecución
        try
        {

            // Query
            var emails = await (from E in context.DataBase.Applications
                                where E.AccountID == id
                                select E).ToListAsync();

            return new(Responses.Success, emails);
        }
        catch
        {
        }

        return new();
    }




    /// <summary>
    /// Obtiene una app
    /// </summary>
    /// <param name="id">ID de la app</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadOneResponse<ApplicationModel>> Read(int id, Conexión context)
    {

        // Ejecución
        try
        {

            // Query
            var email = await (from E in context.DataBase.Applications
                               where E.ID == id
                               select E).FirstOrDefaultAsync();

            // Email no existe
            if (email == null)
            {
                return new(Responses.NotRows);
            }

            return new(Responses.Success, email);
        }
        catch
        {
        }

        return new();
    }



    /// <summary>
    /// Obtiene una app
    /// </summary>
    /// <param name="key">Key de la app</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadOneResponse<ApplicationModel>> Read(string key, Conexión context)
    {

        // Ejecución
        try
        {

            // Query
            var email = await (from E in context.DataBase.Applications
                               where E.Key == key
                               select E).FirstOrDefaultAsync();

            // Email no existe
            if (email == null)
            {
                return new(Responses.NotRows);
            }

            return new(Responses.Success, email);
        }
        catch
        {
        }

        return new();
    }



    /// <summary>
    /// Obtiene una app
    /// </summary>
    /// <param name="uid">UId de la app</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadOneResponse<ApplicationModel>> ReadByAppUid(string uid, Conexión context)
    {

        // Ejecución
        try
        {

            // Query
            var app = await (from E in context.DataBase.Applications
                             where E.ApplicationUid == uid
                             select E).FirstOrDefaultAsync();

            // Email no existe
            if (app == null)
                return new(Responses.NotRows);

            return new(Responses.Success, app);
        }
        catch
        {
        }

        return new();
    }


}