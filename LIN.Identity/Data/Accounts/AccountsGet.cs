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
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

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
    /// Obtiene una cuenta y trae info extra si es de la org
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="orgID">ID de la org</param>
    /// <param name="context">Contexto</param>
    public static async Task<ReadOneResponse<AccountModel>> Read(int id, int contextUser, int orgID, Conexión context)
    {

        // Ejecución
        try
        {

            var query = Queries.Accounts.GetStableAccount(id, contextUser, orgID, true, context);

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
    /// Obtiene una cuenta y trae info extra si es de la org
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="orgID">ID de la org</param>
    /// <param name="context">Contexto</param>
    public static async Task<ReadOneResponse<AccountModel>> Read(string user, int contextUser, int orgID, Conexión context)
    {

        // Ejecución
        try
        {

            var query = Queries.Accounts.GetStableAccount(user, contextUser, orgID, true, context);

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
    /// Obtiene una cuenta y trae info extra si es de la org
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="orgID">ID de la org</param>
    /// <param name="context">Contexto</param>
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
    /// Obtiene una cuenta y trae info extra si es de la org
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="orgID">ID de la org</param>
    /// <param name="context">Contexto</param>
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
    /// Obtiene una lista de diez (10) usuarios que coincidan con un patron
    /// </summary>
    /// <param name="pattern">Patron de búsqueda</param>
    /// <param name="me">Mi ID</param>
    /// <param name="isAdmin">Si es un admin del sistema el que esta consultando</param>
    /// <param name="context">Contexto de conexión</param>
    public static async Task<ReadAllResponse<AccountModel>> Search(string pattern, int me, int orgID, bool isAdmin, Conexión context)
    {

        // Ejecución
        try
        {

            List<AccountModel> accountModels = new List<AccountModel>();

            if (isAdmin)
                accountModels = await Queries.Accounts.Search(pattern, me, orgID, false, context).Take(10).ToListAsync();
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
    /// Obtiene una lista de usuarios por medio del ID
    /// </summary>
    /// <param name="ids">Lista de IDs</param>
    /// <param name="org">ID de organización</param>
    /// <param name="context">Contexto de conexión</param>
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