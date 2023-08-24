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

        var res = await SearchByPattern(pattern, id, context);
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
    /// <param name="safeFilter">TRUE para solo obtener usuarios activos</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadOneResponse<AccountModel>> Read(int id, bool safeFilter, bool privateInfo, bool includeOrg, Conexión context)
    {

        // Ejecución
        try
        {




            // Consulta global
            var query = from A in context.DataBase.Accounts
                        where A.ID == id
                        select A;


            if (includeOrg)
            {
                query = query.Include(a => a.OrganizationAccess).ThenInclude(a => a.Organization);
            }


            // Filtro seguro
            {
                query = query.Where(T => T.Estado == AccountStatus.Normal);
                query = Filters.Account.Get(query);
            }


            // Si no necesita información privada
            if (!privateInfo)
                query = Filters.Account.FilterInfoIf(query);


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
    /// <param name="safeFilter">TRUE para solo obtener usuarios activos</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadOneResponse<AccountModel>> Read(string user, bool safeFilter, bool privateInfo, bool includeOrg, Conexión context)
    {

        // Ejecución
        try
        {

            // Consulta global
            var query = from A in context.DataBase.Accounts
                        where A.Usuario == user
                        select A;


            if (includeOrg)
            {
                query = query.Include(a => a.OrganizationAccess).ThenInclude(a => a.Organization).ThenInclude(a => a.AppList).ThenInclude(a => a.App);
            }


            // Filtro seguro
            if (safeFilter)
            {
                query = query.Where(T => T.Estado == AccountStatus.Normal);
                query = Filters.Account.Get(query);
            }

            // Si no necesita información privada
            if (!privateInfo)
                query = Filters.Account.FilterInfoIf(query);

            // Trae la cuenta
            var account = await query.FirstOrDefaultAsync();

            // Si no existe el modelo
            if (account == null)
                return new(Responses.NotExistAccount);

            return new(Responses.Success, account);

        }
        catch
        {
        }

        return new();
    }



    /// <summary>
    /// Obtiene los primeros 10 usuarios que coincidan con el patron
    /// </summary>
    /// <param name="pattern">Patron a buscar</param>
    /// <param name="id">ID de la cuenta (Contexto)</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadAllResponse<AccountModel>> SearchByPattern(string pattern, int id, Conexión context)
    {

        // Ejecución
        try
        {

            // Query
            var query = (from A in context.DataBase.Accounts
                         where A.Usuario.ToLower().Contains(pattern.ToLower())
                         && A.ID != id
                         && A.Visibilidad == AccountVisibility.Visible
                         select A).Take(10);

            // Ejecuta
            var result = await query.Select(a => new AccountModel
            {
                ID = a.ID,
                Nombre = a.Nombre,
                Usuario = a.Usuario,
                Perfil = a.Perfil,
                Genero = a.Genero,
                Insignia = a.Insignia
            }).ToListAsync();


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
    /// 
    /// </summary>
    /// <param name="pattern">Patron a buscar</param>
    /// <param name="id">ID de la cuenta (Contexto)</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadAllResponse<AccountModel>> FindAll(List<int> ids, Conexión context)
    {

        // Ejecución
        try
        {

            // Query
            var query = from A in context.DataBase.Accounts
                         where A.Estado == AccountStatus.Normal
                         where ids.Contains(A.ID)
                         select A;

            // Ejecuta
            var result = await Filters.Account.FilterInfoIf(query).ToListAsync();

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
    /// Obtiene las primeros 5 cuentas que coincidan con el patron (Admin)
    /// </summary>
    /// <param name="pattern">Patron a buscar</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadAllResponse<AccountModel>> GetAll(string pattern, Conexión context)
    {

        // Ejecución
        try
        {
            var res = await context.DataBase.Accounts
                .Where(T => T.Usuario.ToLower().Contains(pattern.ToLower()))
                .Take(5)
                .ToListAsync();

            // Si no existe el modelo
            if (res == null)
                return new(Responses.NotRows);

            return new(Responses.Success, res);
        }
        catch
        {
        }

        return new();
    }



}