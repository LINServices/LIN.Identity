namespace LIN.Identity.Data;


public class Applications
{


    #region Abstracciones


    /// <summary>
    /// Crea una app
    /// </summary>
    /// <param name="data">Modelo</param>
    public static async Task<CreateResponse> Create(ApplicationModel data)
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
    public static async Task<ReadAllResponse<ApplicationModel>> ReadAll(int id)
    {
        var (context, contextKey) = Conexión.GetOneConnection();

        var res = await ReadAll(id, context);
        context.CloseActions(contextKey);
        return res;
    }




    /// <summary>
    /// Agregar una identidad al directorio de una app.
    /// </summary>
    /// <param name="appId">Id de la app.</param>
    /// <param name="accountId">Id de la identidad.</param>
    public static async Task<ReadOneResponse<bool>> AllowTo(int appId, int accountId)
    {
        var (context, contextKey) = Conexión.GetOneConnection();

        var res = await AllowTo(appId, accountId, context);
        context.CloseActions(contextKey);
        return res;
    }






    /// <summary>
    /// Obtiene una app
    /// </summary>
    /// <param name="id">ID de la app</param>
    public static async Task<ReadOneResponse<ApplicationModel>> Read(int id)
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
    public static async Task<ReadOneResponse<ApplicationModel>> Read(string key)
    {
        var (context, contextKey) = Conexión.GetOneConnection();

        var res = await Read(key, context);
        context.CloseActions(contextKey);
        return res;
    }


    #endregion



    /// <summary>
    /// Crear aplicación.
    /// </summary>
    /// <param name="data">Modelo</param>
    /// <param name="context">Contexto de conexión</param>
    public static async Task<CreateResponse> Create(ApplicationModel data, Conexión context)
    {

        // ID.
        data.ID = 0;

        // Ejecución
        try
        {

            // Guardar la información.
            await context.DataBase.Applications.AddAsync(data);

            // Llevar info a la BD.
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
    /// Obtiene la lista de apps asociados a una cuenta.
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="context">Contexto de conexión</param>
    public static async Task<ReadAllResponse<ApplicationModel>> ReadAll(int id, Conexión context)
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
    /// Obtiene una app.
    /// </summary>
    /// <param name="id">ID de la app</param>
    /// <param name="context">Contexto de conexión</param>
    public static async Task<ReadOneResponse<ApplicationModel>> Read(int id, Conexión context)
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
                return new(Responses.NotRows);

            // Correcto.
            return new(Responses.Success, email);
        }
        catch (Exception)
        {
        }

        return new();
    }



    /// <summary>
    /// Obtiene una app
    /// </summary>
    /// <param name="key">Key de la app</param>
    /// <param name="context">Contexto de conexión</param>
    public static async Task<ReadOneResponse<ApplicationModel>> Read(string key, Conexión context)
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
                return new(Responses.NotRows);


            return new(Responses.Success, email);
        }
        catch (Exception)
        {
        }

        return new();
    }



    /// <summary>
    /// Obtiene una app
    /// </summary>
    /// <param name="uid">UId de la app</param>
    /// <param name="context">Contexto de conexión</param>
    public static async Task<ReadOneResponse<ApplicationModel>> ReadByAppUid(string uid, Conexión context)
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
        catch (Exception)
        {
        }

        return new();
    }



    /// <summary>
    /// Permitir acceso a una identidad al directorio de una app.
    /// </summary>
    /// <param name="appId">Id de la app.</param>
    /// <param name="identityId">Id de la identidad.</param>
    /// <param name="context">Cuenta de conexión.</param>
    public static async Task<ReadOneResponse<bool>> AllowTo(int appId, int identityId, Conexión context)
    {

        // Ejecución
        try
        {


            var application = await (from app in context.DataBase.Applications
                                     where app.ID == appId
                                     select new ApplicationModel()
                                     {
                                         ID = app.ID,
                                         DirectoryId = app.DirectoryId
                                     }).FirstOrDefaultAsync();

            if (application == null)
            {
                return new()
                {
                    Response = Responses.NotRows
                };
            }


            if (application.DirectoryId <= 0)
            {
                return new()
                {
                    Response = Responses.NotFoundDirectory
                };
            }

            DirectoryMember member = new()
            {
                Identity = new IdentityModel()
                {
                    Id = identityId
                },
                Directory = new()
                {
                    ID = application.DirectoryId,
                }
            };
            context.DataBase.Attach(member.Identity);
            context.DataBase.Attach(member.Directory);

            context.DataBase.DirectoryMembers.Add(member);

            context.DataBase.SaveChanges();

            return new(Responses.Success, true);
        }
        catch
        {
        }

        return new();
    }



}