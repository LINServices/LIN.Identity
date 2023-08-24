namespace LIN.Auth.Data.Accounts;


public static partial class Accounts
{


    #region Abstracciones





    /// <summary>
    /// Obtiene un usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    public async static Task<ReadOneResponse<AccountModel>> Read(int id, bool safeFilter, bool privateInfo = true, bool includeOrg = false)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await Read(id, safeFilter, privateInfo, includeOrg, context);
        context.CloseActions(connectionKey);
        return res;

    }



    /// <summary>
    /// Obtiene un usuario
    /// </summary>
    /// <param name="user">Usuario de la cuenta</param>
    public async static Task<ReadOneResponse<AccountModel>> Read(string user, bool safeFilter, bool privateInfo = true, bool includeOrg = false)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await Read(user, safeFilter, privateInfo, includeOrg, context);
        context.CloseActions(connectionKey);
        return res;
    }



    /// <summary>
    /// Obtiene los primeros 10 usuarios que coincidan con el patron
    /// </summary>
    /// <param name="pattern">Patron a buscar</param>
    /// <param name="id">ID de la cuenta</param>
    public async static Task<ReadAllResponse<AccountModel>> SearchByPattern(string pattern, int id)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await Search(pattern, id, context);
        context.CloseActions(connectionKey);
        return res;
    }




    /// <summary>
    /// Obtiene la lista de usuarios correspondiente a los ids
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    public async static Task<ReadAllResponse<AccountModel>> FindAll(List<int> ids)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await FindAll(ids, context);
        context.CloseActions(connectionKey);
        return res;
    }



    /// <summary>
    /// Obtiene los primeros 5 usuarios que coincidan con el patron (ADMIN)
    /// </summary>
    /// <param name="pattern">Patron a buscar</param>
    public async static Task<ReadAllResponse<AccountModel>> GetAll(string pattern)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await GetAll(pattern, context);
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
    public async static Task<ReadOneResponse<AccountModel>> Read(int id, bool safeFilter, bool includePrivateInfo, bool includeOrg, Conexión context)
    {

        // Ejecución
        try
        {

            // Consulta global
            var query = from A in context.DataBase.Accounts
                        where A.ID == id
                        select A;

            // Armar la consulta final
            query = Filters.Account.Filter(query, safeFilter, includeOrg, includePrivateInfo);

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
    public async static Task<ReadOneResponse<AccountModel>> Read(string user, bool safeFilter, bool includePrivateInfo, bool includeOrg, Conexión context)
    {

        // Ejecución
        try
        {

            // Consulta global
            var query = from A in context.DataBase.Accounts
                        where A.Usuario == user
                        select A;

            // Armar la consulta final
            query = Filters.Account.Filter(query, safeFilter, includeOrg, includePrivateInfo);

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
                query = Filters.Account.Filter(baseQuery: query,
                                               safe: false, includeOrg: true, privateInfo: false);
            
            else
                query = Filters.Account.Filter(baseQuery: query,
                                                   safe: false, includeOrg: true,
                                                   privateInfo: false);


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
    public async static Task<ReadAllResponse<AccountModel>> FindAll(List<int> ids, int org, Conexión context)
    {

        // Ejecución
        try
        {

            // Query
            var query = from A in context.DataBase.Accounts
                        where ids.Contains(A.ID)
                        select A;

            // Si hay que incluir la query de organización
            var privateInformation = org > 0;

            // Solo participantes de una organización
            if (privateInformation)
                query = from A in query
                        where A.OrganizationAccess.Organization.ID == org
                        select A;

            // Armar la consulta final
            query = Filters.Account.Filter(query, true, false, privateInformation);


            // Ejecuta
            var result = await query.Take(10).ToListAsync();

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



}