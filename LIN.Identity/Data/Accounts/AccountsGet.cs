using LIN.Identity.Data.Filters;

namespace LIN.Identity.Data;


public static partial class Accounts
{


    #region Abstracciones



    /// <summary>
    /// Obtiene una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="safeFilter">Filtro seguro</param>
    /// <param name="includePrivateInfo">Filtro de información privada</param>
    /// <param name="includeOrg">Incluir organización</param>
    public async static Task<ReadOneResponse<AccountModel>> Read(int id, bool safeFilter, bool includePrivateInfo = true, bool includeOrg = false, bool sensible = false)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await Read(id, safeFilter, includePrivateInfo, includeOrg, sensible, context);
        context.CloseActions(connectionKey);
        return res;

    }


    /// <summary>
    /// Obtiene una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="orgID">Info privada si la org es igual a OrgID</param>
    public async static Task<ReadOneResponse<AccountModel>> Read(int id, int orgID)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await Read(id,orgID, context);
        context.CloseActions(connectionKey);
        return res;

    }



    /// <summary>
    /// Obtiene una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="orgID">Info privada si la org es igual a OrgID</param>
    public async static Task<ReadOneResponse<AccountModel>> Read(string user, int orgID)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await Read(user, orgID, context);
        context.CloseActions(connectionKey);
        return res;

    }


    /// <summary>
    /// Obtiene una cuenta
    /// </summary>
    /// <param name="user">Usuario de la cuenta</param>
    /// <param name="safeFilter">Filtro seguro</param>
    /// <param name="includePrivateInfo">Filtro de información privada</param>
    /// <param name="includeOrg">Incluir organización</param>
    public async static Task<ReadOneResponse<AccountModel>> Read(string user, bool safeFilter, bool includePrivateInfo = true, bool includeOrg = false, bool sensible = false)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await Read(user, safeFilter, includePrivateInfo, includeOrg, sensible, context);
        context.CloseActions(connectionKey);
        return res;
    }





    /// <summary>
    /// Obtiene una lista de diez (10) usuarios que coincidan con un patron
    /// </summary>
    /// <param name="pattern">Patron de búsqueda</param>
    /// <param name="me">Mi ID</param>
    /// <param name="isAdmin">Si es un admin del sistema el que esta consultando</param>
    public async static Task<ReadAllResponse<AccountModel>> Search(string pattern, int me, bool isAdmin = false)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await Search(pattern, me, isAdmin, context);
        context.CloseActions(connectionKey);
        return res;
    }



    /// <summary>
    /// Obtiene una lista de usuarios por medio del ID
    /// </summary>
    /// <param name="ids">Lista de IDs</param>
    /// <param name="org">ID de organización</param>
    public async static Task<ReadAllResponse<AccountModel>> FindAll(List<int> ids, int me = 0, int org = 0)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await FindAll(ids,me, org, context);
        context.CloseActions(connectionKey);
        return res;
    }



    #endregion



    /// <summary>
    /// Obtiene una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="safeFilter">Filtro seguro</param>
    /// <param name="includePrivateInfo">Filtro de información privada</param>
    /// <param name="includeOrg">Incluir organización</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadOneResponse<AccountModel>> Read(int id, bool safeFilter, bool includePrivateInfo, bool includeOrg, bool sensible, Conexión context)
    {

        // Ejecución
        try
        {

            // Consulta global
            var query = from A in context.DataBase.Accounts
                        where A.ID == id
                        select A;

            // Armar la consulta final
            query = Account.Filter(query, safeFilter, includeOrg, includePrivateInfo, sensible);

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
    public async static Task<ReadOneResponse<AccountModel>> Read(int id, int orgID, Conexión context)
    {

        // Ejecución
        try
        {

            // Consulta global
            var query = from A in context.DataBase.Accounts
                        where A.ID == id
                        select A;

            // Armar la consulta final
            query = Account.Filter(query, orgID);

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
    public async static Task<ReadOneResponse<AccountModel>> Read(string user, int orgID, Conexión context)
    {

        // Ejecución
        try
        {

            // Consulta global
            var query = from A in context.DataBase.Accounts
                        where A.Usuario == user
                        select A;

            // Armar la consulta final
            query = Account.Filter(query, orgID);

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
    /// Obtiene una cuenta
    /// </summary>
    /// <param name="user">Usuario de la cuenta</param>
    /// <param name="safeFilter">Filtro seguro</param>
    /// <param name="includePrivateInfo">Filtro de información privada</param>
    /// <param name="includeOrg">Incluir organización</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadOneResponse<AccountModel>> Read(string user, bool safeFilter, bool includePrivateInfo, bool includeOrg, bool sensible, Conexión context)
    {

        // Ejecución
        try
        {

            // Consulta global
            var query = from A in context.DataBase.Accounts
                        where A.Usuario == user
                        select A;

            // Armar la consulta final
            query = Account.Filter(query, safeFilter, includeOrg, includePrivateInfo, sensible);

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
    public async static Task<ReadAllResponse<AccountModel>> Search(string pattern, int me, bool isAdmin, Conexión context)
    {

        // Ejecución
        try
        {

            // Query
            var query = (from A in context.DataBase.Accounts
                         where A.Usuario.ToLower().Contains(pattern.ToLower())
                         && A.ID != me
                         select A).Take(10);

            // Armar la consulta
            if (isAdmin)
                query = Account.Filter(baseQuery: query,
                                               safe: false, includeOrg: true, privateInfo: false, sensible: true);

            else
                query = Account.Filter(baseQuery: query,
                                                   safe: false, includeOrg: true,
                                                   privateInfo: false, sensible: false);


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
    /// Obtiene una lista de usuarios por medio del ID
    /// </summary>
    /// <param name="ids">Lista de IDs</param>
    /// <param name="org">ID de organización</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadAllResponse<AccountModel>> FindAll(List<int> ids, int me, int org, Conexión context)
    {

        // Ejecución
        try
        {

            // Consulta base
            var query = from A in context.DataBase.Accounts
                        where ids.Contains(A.ID)
                        select A;

            query = Account.Filter(query, org, me);

            // Ejecuta
            var result = await query.ToListAsync();

            // Si no existe el modelo
            if (result == null)
                return new(Responses.NotRows);

            return new(Responses.Success, result);
        }
        catch(Exception ex)
        {
            var s = "";
        }

        return new();
    }



}