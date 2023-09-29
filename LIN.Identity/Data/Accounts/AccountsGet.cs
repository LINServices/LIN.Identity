namespace LIN.Identity.Data;


internal static partial class Accounts
{


    #region Abstracciones


    /// <summary>
    /// Obtiene una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="contextUser">ID del usuario contexto</param>
    /// <param name="orgId">Info privada si la org es igual a OrgID</param>
    public static async Task<ReadOneResponse<AccountModel>> Read(int id, int contextUser, int orgId)
    {

        // Obtiene la conexión
        var (context, connectionKey) = Conexión.GetOneConnection();

        var res = await Read(id, contextUser, orgId, context);
        context.CloseActions(connectionKey);
        return res;

    }



    /// <summary>
    /// Obtiene una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="includeOrg">Incluir la organización</param>
    public static async Task<ReadOneResponse<AccountModel>> Read(int id, bool includeOrg)
    {

        // Obtiene la conexión
        var (context, connectionKey) = Conexión.GetOneConnection();

        var res = await Read(id, includeOrg, context);
        context.CloseActions(connectionKey);
        return res;

    }



    /// <summary>
    /// Obtiene una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="includeOrg">Incluir la organización</param>
    public static async Task<ReadOneResponse<AccountModel>> Read(string id, bool includeOrg)
    {

        // Obtiene la conexión
        var (context, connectionKey) = Conexión.GetOneConnection();

        var res = await Read(id, includeOrg, context);
        context.CloseActions(connectionKey);
        return res;

    }



    /// <summary>
    /// Obtiene la informacion basica de un usuario
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
    /// Obtiene la informacion basica de un usuario
    /// </summary>
    /// <param name="user">Usuario unico</param>
    public static async Task<ReadOneResponse<AccountModel>> ReadBasic(string user)
    {

        // Obtiene la conexión
        var (context, connectionKey) = Conexión.GetOneConnection();

        var res = await ReadBasic(user, context);
        context.CloseActions(connectionKey);
        return res;

    }



    /// <summary>
    /// Obtiene una cuenta
    /// </summary>
    /// <param name="user">Usuario único</param>
    /// <param name="contextUser">Usuario de contexto</param>
    /// <param name="orgId">Info privada si la org es igual a OrgID</param>
    public static async Task<ReadOneResponse<AccountModel>> Read(string user, int contextUser, int orgId)
    {

        // Obtiene la conexión
        var (context, connectionKey) = Conexión.GetOneConnection();

        var res = await Read(user, contextUser, orgId, context);
        context.CloseActions(connectionKey);
        return res;

    }



    /// <summary>
    /// Obtiene una lista de diez (10) usuarios que coincidan con un patron
    /// </summary>
    /// <param name="pattern">Patron de búsqueda</param>
    /// <param name="me">Mi ID</param>
    /// <param name="orgId">ID de la org de contexto</param>
    public static async Task<ReadAllResponse<AccountModel>> Search(string pattern, int me, int orgId)
    {

        // Obtiene la conexión
        var (context, connectionKey) = Conexión.GetOneConnection();

        var res = await Search(pattern, me, orgId, false, context);
        context.CloseActions(connectionKey);
        return res;
    }



    /// <summary>
    /// Obtiene una lista de diez (10) usuarios que coincidan con un patron
    /// </summary>
    /// <param name="pattern">Patron de búsqueda</param>
    public static async Task<ReadAllResponse<AccountModel>> Search(string pattern)
    {

        // Obtiene la conexión
        var (context, connectionKey) = Conexión.GetOneConnection();

        var res = await Search(pattern, 0, 0, false, context);
        context.CloseActions(connectionKey);
        return res;
    }



    /// <summary>
    /// Obtiene una lista de usuarios por medio del ID
    /// </summary>
    /// <param name="ids">Lista de IDs</param>
    /// <param name="me">ID del usuario contexto</param>
    /// <param name="org">ID de organización</param>
    public static async Task<ReadAllResponse<AccountModel>> FindAll(List<int> ids, int me = 0, int org = 0)
    {

        // Obtiene la conexión
        var (context, connectionKey) = Conexión.GetOneConnection();

        var res = await FindAll(ids, me, org, context);
        context.CloseActions(connectionKey);
        return res;
    }


    #endregion


    /// <summary>
    /// Obtiene un usuario y trae informacion extra (Organizacion y datos privados) segun el contexto de
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="contextUser">Usuario que solicita</param>
    /// <param name="orgId">Organizacion que solicita</param>
    /// <param name="context">Contexto de base de datos</param>
    public static async Task<ReadOneResponse<AccountModel>> Read(int id, int contextUser, int orgId, Conexión context)
    {

        // Ejecución
        try
        {

            var query = Queries.Accounts.GetStableAccount(id, contextUser, orgId, true, context);

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
    /// Obtiene un usuario y trae informacion extra (Organizacion y datos privados) segun el contexto de
    /// </summary>
    /// <param name="user">Usuario unico</param>
    /// <param name="contextUser">Usuario que solicita</param>
    /// <param name="orgId">Organizacion que solicita</param>
    /// <param name="context">Contexto de base de datos</param>
    public static async Task<ReadOneResponse<AccountModel>> Read(string user, int contextUser, int orgId, Conexión context)
    {

        // Ejecución
        try
        {

            var query = Queries.Accounts.GetStableAccount(user, contextUser, orgId, true, context);

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
    /// <param name="user">ID del usuario</param>
    /// <param name="includeOrg">Incluir la organizacion</param>
    /// <param name="context">Contexto de base de datos</param>
    public static async Task<ReadOneResponse<AccountModel>> Read(int user, bool includeOrg, Conexión context)
    {

        // Ejecución
        try
        {

            var query = Queries.Accounts.GetStableAccount(user, includeOrg, context);

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
    /// <param name="user">Usuario unico de la cuenta</param>
    /// <param name="includeOrg">Incluir la organizacion</param>
    /// <param name="context">Contexto de base de datos</param>
    public static async Task<ReadOneResponse<AccountModel>> Read(string user, bool includeOrg, Conexión context)
    {

        // Ejecución
        try
        {

            var query = Queries.Accounts.GetStableAccount(user, includeOrg, context);

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
    /// Buscar usuarios por patron de busqueda.
    /// </summary>
    /// <param name="pattern">Patron de busqueda</param>
    /// <param name="me">ID del usuario contexto</param>
    /// <param name="orgId">ID de la organizacion de contexto</param>
    /// <param name="isAdmin">Es administrador</param>
    /// <param name="context">Contexto de base de datos</param>
    public static async Task<ReadAllResponse<AccountModel>> Search(string pattern, int me, int orgId, bool isAdmin, Conexión context)
    {

        // Ejecución
        try
        {

            List<AccountModel> accountModels = new();

            if (isAdmin)
                accountModels = await Queries.Accounts.Search(pattern, me, orgId, false, context).Take(10).ToListAsync();
            else
            {
                accountModels = await Queries.Accounts.SearchOnAll(pattern, true, context).Take(10).ToListAsync();
            }

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
    /// <param name="org">ID de la organizacion de contexto</param>
    /// <param name="context">Contexto de base de datos</param>
    public static async Task<ReadAllResponse<AccountModel>> FindAll(List<int> ids, int me, int org, Conexión context)
    {

        // Ejecución
        try
        {

            var query = Queries.Accounts.GetStableAccounts(ids, me, org, true, context);

            // Ejecuta
            var result = await query.ToListAsync();

            // Si no existe el modelo
            if (result == null)
                return new(Responses.NotRows);

            return new(Responses.Success, result);
        }
        catch (Exception ex)
        {
            var s = "";
        }

        return new();
    }


    /// <summary>
    /// Obtiene la informacion basica de un usuario
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
                            Usuario = account.Usuario,
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
    /// Obtiene la informacion basica de un usuario
    /// </summary>
    /// <param name="user">Usuario unico</param>
    /// <param name="context">Contexto de base de datos</param>
    public static async Task<ReadOneResponse<AccountModel>> ReadBasic(string user, Conexión context)
    {

        // Ejecución
        try
        {

            var query = from account in Queries.Accounts.GetValidAccounts(context)
                        where account.Usuario == user
                        select new AccountModel
                        {
                            ID = account.ID,
                            Usuario = account.Usuario,
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