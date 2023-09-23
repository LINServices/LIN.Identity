namespace LIN.Identity.Data;


public static partial class Accounts
{


    #region Abstracciones






    /// <summary>
    /// Obtiene una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="orgID">Info privada si la org es igual a OrgID</param>
    public async static Task<ReadOneResponse<AccountModel>> Read(int id, int contextUser, int orgID)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await Read(id, contextUser, orgID, context);
        context.CloseActions(connectionKey);
        return res;

    }



    public async static Task<ReadOneResponse<AccountModel>> ReadBasic(int id)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await ReadBasic(id, context);
        context.CloseActions(connectionKey);
        return res;

    }



    public async static Task<ReadOneResponse<AccountModel>> ReadBasic(string user)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await ReadBasic(user, context);
        context.CloseActions(connectionKey);
        return res;

    }



    /// <summary>
    /// Obtiene una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="orgID">Info privada si la org es igual a OrgID</param>
    public async static Task<ReadOneResponse<AccountModel>> Read(string user, int contextUser, int orgID)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await Read(user, contextUser, orgID, context);
        context.CloseActions(connectionKey);
        return res;

    }







    /// <summary>
    /// Obtiene una lista de diez (10) usuarios que coincidan con un patron
    /// </summary>
    /// <param name="pattern">Patron de búsqueda</param>
    /// <param name="me">Mi ID</param>
    /// <param name="isAdmin">Si es un admin del sistema el que esta consultando</param>
    public async static Task<ReadAllResponse<AccountModel>> Search(string pattern, int me, int orgID)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await Search(pattern, me, orgID, context);
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
    public async static Task<ReadOneResponse<AccountModel>> Read(int id, int contextUser, int orgID, Conexión context)
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
    public async static Task<ReadOneResponse<AccountModel>> Read(string user, int contextUser, int orgID, Conexión context)
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
    /// Obtiene una lista de diez (10) usuarios que coincidan con un patron
    /// </summary>
    /// <param name="pattern">Patron de búsqueda</param>
    /// <param name="me">Mi ID</param>
    /// <param name="isAdmin">Si es un admin del sistema el que esta consultando</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadAllResponse<AccountModel>> Search(string pattern, int me, int orgID, Conexión context)
    {

        // Ejecución
        try
        {

            // Query
            var query = await Queries.Accounts.Search(pattern, me, orgID, false, context).Take(10).ToListAsync();


            // Si no existe el modelo
            if (query == null)
                return new(Responses.NotRows);

            return new(Responses.Success, query);
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






    public async static Task<ReadOneResponse<AccountModel>> ReadBasic(int id, Conexión context)
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


    public async static Task<ReadOneResponse<AccountModel>> ReadBasic(string user, Conexión context)
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