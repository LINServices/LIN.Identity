namespace LIN.Identity.Data;


internal static partial class Accounts
{


    #region Abstracciones


    public static async Task<ReadOneResponse<AccountModel>> Read(int id, FilterModels.Account filters)
    {

        // Obtiene la conexión
        var (context, connectionKey) = Conexión.GetOneConnection();

        var res = await Read(id, filters, context);
        context.CloseActions(connectionKey);
        return res;

    }






    /// <summary>
    /// Obtiene una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="includeOrg">Incluir la organización</param>
    public static async Task<ReadOneResponse<AccountModel>> Read(string user, FilterModels.Account filters)
    {

        // Obtiene la conexión
        var (context, connectionKey) = Conexión.GetOneConnection();

        var res = await Read(user, filters, context);
        context.CloseActions(connectionKey);
        return res;

    }



    /// <summary>
    /// Obtiene la información básica de un usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    public static async Task<ReadOneResponse<AccountModel>> ReadBasic(int id)
    {

        // Obtiene la conexión
        var (context, connectionKey) = Conexión.GetOneConnection();

        var res = await ReadBasic(id, context);
        context.CloseActions(connectionKey);
        return res;

    }



    /// <summary>
    /// Obtiene la información básica de un usuario
    /// </summary>
    /// <param name="user">Usuario único</param>
    public static async Task<ReadOneResponse<AccountModel>> ReadBasic(string user)
    {

        // Obtiene la conexión
        var (context, connectionKey) = Conexión.GetOneConnection();

        var res = await ReadBasic(user, context);
        context.CloseActions(connectionKey);
        return res;

    }




    /// <summary>
    /// Obtiene una lista de diez (10) usuarios que coincidan con un patron
    /// </summary>
    /// <param name="pattern">Patron de búsqueda</param>
    /// <param name="me">Mi ID</param>
    /// <param name="orgId">ID de la org de contexto</param>
    public static async Task<ReadAllResponse<AccountModel>> Search(string pattern, FilterModels.Account filters)
    {

        // Obtiene la conexión
        var (context, connectionKey) = Conexión.GetOneConnection();

        var res = await Search(pattern, filters, context);
        context.CloseActions(connectionKey);
        return res;
    }






    /// <summary>
    /// Obtiene una lista de usuarios por medio del ID
    /// </summary>
    /// <param name="ids">Lista de IDs</param>
    /// <param name="me">ID del usuario contexto</param>
    /// <param name="org">ID de organización</param>
    public static async Task<ReadAllResponse<AccountModel>> FindAll(List<int> ids, FilterModels.Account filters)
    {

        // Obtiene la conexión
        var (context, connectionKey) = Conexión.GetOneConnection();

        var res = await FindAll(ids, filters, context);
        context.CloseActions(connectionKey);
        return res;
    }


    #endregion




    /// <summary>
    /// Obtiene un usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="filters">Filtros de búsqueda</param>
    /// <param name="context">Contexto de base de datos</param>
    public static async Task<ReadOneResponse<AccountModel>> Read(int id, FilterModels.Account filters, Conexión context)
    {

        // Ejecución
        try
        {

            var query = Queries.Accounts.GetAccounts(id, filters, context);

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



    /// <summary>
    /// Obtiene un usuario
    /// </summary>
    /// <param name="user">Usuario único</param>
    /// <param name="filters">Filtros de búsqueda</param>
    /// <param name="context">Contexto de base de datos</param>
    public static async Task<ReadOneResponse<AccountModel>> Read(string user, FilterModels.Account filters, Conexión context)
    {

        // Ejecución
        try
        {

            var query = Queries.Accounts.GetAccounts(user, filters, context);

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




    /// <summary>
    /// Buscar usuarios por patron de búsqueda.
    /// </summary>
    /// <param name="pattern">Patron de búsqueda</param>
    /// <param name="me">ID del usuario contexto</param>
    /// <param name="orgId">ID de la organización de contexto</param>
    /// <param name="isAdmin">Es administrador</param>
    /// <param name="context">Contexto de base de datos</param>
    public static async Task<ReadAllResponse<AccountModel>> Search(string pattern, FilterModels.Account filters, Conexión context)
    {

        // Ejecución
        try
        {

            List<AccountModel> accountModels = await Queries.Accounts.Search(pattern, filters, context).Take(10).ToListAsync();

            // Si no existe el modelo
            if (accountModels == null)
                return new(Responses.NotRows);

            return new(Responses.Success, accountModels);
        }
        catch
        {
        }

        return new();
    }



    /// <summary>
    /// Obtiene los usuarios con IDs coincidentes
    /// </summary>
    /// <param name="ids">Lista de IDs</param>
    /// <param name="me">ID del usuario contexto</param>
    /// <param name="org">ID de la organización de contexto</param>
    /// <param name="context">Contexto de base de datos</param>
    public static async Task<ReadAllResponse<AccountModel>> FindAll(List<int> ids, FilterModels.Account filters, Conexión context)
    {

        // Ejecución
        try
        {

            var query = Queries.Accounts.GetAccounts(ids, filters, context);

            // Ejecuta
            var result = await query.ToListAsync();

            // Si no existe el modelo
            if (result == null)
                return new(Responses.NotRows);

            return new(Responses.Success, result);
        }
        catch
        {
        }

        return new();
    }



    /// <summary>
    /// Obtiene la información básica de un usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="context">Contexto de base de datos</param>
    public static async Task<ReadOneResponse<AccountModel>> ReadBasic(int id, Conexión context)
    {

        // Ejecución
        try
        {

            var query = from account in Queries.Accounts.GetValidAccounts(context)
                        where account.ID == id
                        select new AccountModel
                        {
                            ID = account.ID,
                            Identity = new()
                            {
                                Id = account.Identity.Id,
                                Type = account.Identity.Type,
                                Unique = account.Identity.Unique
                            },
                            Contraseña = account.Contraseña,
                            Estado = account.Estado,
                            Nombre = account.Nombre,
                            OrganizationAccess = account.OrganizationAccess
                        };

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



    /// <summary>
    /// Obtiene la información básica de un usuario
    /// </summary>
    /// <param name="user">Usuario único</param>
    /// <param name="context">Contexto de base de datos</param>
    public static async Task<ReadOneResponse<AccountModel>> ReadBasic(string user, Conexión context)
    {

        // Ejecución
        try
        {

            var query = from account in Queries.Accounts.GetValidAccounts(context)
                        where account.Identity.Unique == user
                        select new AccountModel
                        {
                            ID = account.ID,
                            Identity = new()
                            {
                                Id = account.Identity.Id,
                                Type = account.Identity.Type,
                                Unique = account.Identity.Unique
                            },
                            Contraseña = account.Contraseña,
                            Estado = account.Estado,
                            Nombre = account.Nombre,
                            OrganizationAccess = account.OrganizationAccess
                        };

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