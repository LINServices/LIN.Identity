namespace LIN.Identity.Queries;


public class Accounts
{


    /// <summary>
    /// Query general para todas las cuentas
    /// </summary>
    /// <param name="context">Contexto DB</param>
    public static IQueryable<AccountModel> GetAccounts(Conexión context)
    {
        // Query general
        IQueryable<AccountModel> accounts = from Account in context.DataBase.Accounts
                                            select Account;

        // Retorno
        return accounts;

    }



    /// <summary>
    /// Solo tiene en cuenta cuentas validas
    /// </summary>
    /// <param name="context">Contexto DB</param>
    public static IQueryable<AccountModel> GetValidAccounts(Conexión context)
    {
        // Query general
        IQueryable<AccountModel> accounts = from Account in GetAccounts(context)
                                            where Account.Estado == AccountStatus.Normal
                                            select Account;

        // Retorno
        return accounts;

    }






    /// <summary>
    /// Obtiene una cuenta valida
    /// </summary>
    /// <param name="userID">ID del usuario a obtener</param>
    /// <param name="contextUserID">ID del usuario que busca la información</param>
    /// <param name="contextOrgID">Contexto de organización</param>
    /// <param name="context">Contexto de conexión</param>
    public static IQueryable<AccountModel> GetStableAccount(int userID, int contextUserID, int contextOrgID, bool includeOrg, Conexión context)
    {

        // Query general
        IQueryable<AccountModel> accounts = from account in GetValidAccounts(context)
                                            where account.ID == userID
                                            select account;

        // Armar el modelo
        accounts = BuildModel(accounts, contextUserID, contextOrgID, includeOrg);

        // Retorno
        return accounts;

    }




    /// <summary>
    /// Obtiene una cuenta valida
    /// </summary>
    /// <param name="userID">ID del usuario a obtener</param>
    /// <param name="includeOrg">Incluir la organización</param>
    /// <param name="context">Contexto de conexión</param>
    public static IQueryable<AccountModel> GetStableAccount(int userID, bool includeOrg, Conexión context)
    {

        // Query general
        IQueryable<AccountModel> accounts = from account in GetValidAccounts(context)
                                            where account.ID == userID
                                            select account;

        // Armar el modelo
        accounts = BuildModel(accounts, includeOrg);

        // Retorno
        return accounts;

    }




    /// <summary>
    /// Obtiene una cuenta valida
    /// </summary>
    /// <param name="user">Usuario único</param>
    /// <param name="includeOrg">Incluir la organización</param>
    /// <param name="context">Contexto de conexión</param>
    public static IQueryable<AccountModel> GetStableAccount(string user, bool includeOrg, Conexión context)
    {

        // Query general
        IQueryable<AccountModel> accounts = from account in GetValidAccounts(context)
                                            where account.Usuario == user
                                            select account;

        // Armar el modelo
        accounts = BuildModel(accounts, includeOrg);

        // Retorno
        return accounts;

    }



    /// <summary>
    /// Obtiene una cuenta valida
    /// </summary>
    /// <param name="user">Usuario único</param>
    /// <param name="includeOrg">Incluir la organización</param>
    /// <param name="context">Contexto de conexión</param>
    public static IQueryable<AccountModel> SearchOnAll(string user, bool includeOrg, Conexión context)
    {

        // Query general
        IQueryable<AccountModel> accounts = from account in GetAccounts(context)
                                            where account.Usuario.ToLower().Contains(user)
                                            select account;

        // Armar el modelo
        accounts = BuildModel(accounts, includeOrg);

        // Retorno
        return accounts;

    }




    /// <summary>
    /// Obtiene una cuenta valida
    /// </summary>
    /// <param name="user">Usuario único</param>
    /// <param name="contextUserID">ID del usuario que busca la información</param>
    /// <param name="contextOrgID">Contexto de organización</param>
    /// <param name="context">Contexto de conexión</param>
    public static IQueryable<AccountModel> GetStableAccount(string user, int contextUserID, int contextOrgID, bool includeOrg, Conexión context)
    {

        // Query general
        IQueryable<AccountModel> accounts = from account in GetValidAccounts(context)
                                            where account.Usuario == user
                                            select account;

        // Armar el modelo
        accounts = BuildModel(accounts, contextUserID, contextOrgID, includeOrg);

        // Retorno
        return accounts;

    }



    /// <summary>
    /// Obtiene una lista cuentas valida
    /// </summary>
    /// <param name="ids">IDs de los usuarios a obtener</param>
    /// <param name="contextUserID">ID del usuario que busca la información</param>
    /// <param name="contextOrgID">Contexto de organización</param>
    /// <param name="context">Contexto de conexión</param>
    public static IQueryable<AccountModel> GetStableAccounts(IEnumerable<int> ids, int contextUserID, int contextOrgID, bool includeOrg, Conexión context)
    {

        // Query general
        IQueryable<AccountModel> accounts = from account in GetValidAccounts(context)
                                            where ids.Contains(account.ID)
                                            select account;

        // Armar el modelo
        accounts = BuildModel(accounts, contextUserID, contextOrgID, includeOrg);

        // Retorno
        return accounts;

    }



    /// <summary>
    /// Busca en usuarios activos
    /// </summary>
    /// <param name="pattern">Patron de búsqueda</param>
    /// <param name="contextUserID">ID del usuario que busca la información</param>
    /// <param name="contextOrgID">Contexto de organización</param>
    /// <param name="context">Contexto de conexión</param>
    public static IQueryable<AccountModel> Search(string pattern, int contextUserID, int contextOrgID, bool includeOrg, Conexión context)
    {

        // Query general
        IQueryable<AccountModel> accounts = from account in GetValidAccounts(context)
                                            where account.Usuario.ToLower().Contains(pattern.ToLower())
                                                  && account.ID != contextUserID
                                            select account;

        // Armar el modelo
        accounts = BuildModel(accounts, contextUserID, contextOrgID, includeOrg);

        // Retorno
        return accounts;

    }



    /// <summary>
    /// Armar el modelo final
    /// </summary>
    /// <param name="query">Query base</param>
    /// <param name="contextUserID">Usuario de contexto</param>
    /// <param name="contextOrgID">Organización de contexto</param>
    private static IQueryable<AccountModel> BuildModel(IQueryable<AccountModel> query, int contextUserID, int contextOrgID, bool includeOrg)
    {

        byte[] profile =
        {
        };
        try
        {
            // Imagen genérica
            profile = File.ReadAllBytes("wwwroot/user.png");
        }
        catch { }

        return from account in query
               select new AccountModel
               {
                   ID = account.ID,
                   Nombre = account.Visibilidad == AccountVisibility.Visible || account.OrganizationAccess != null && account.OrganizationAccess.Organization.ID == contextOrgID || account.ID == contextUserID ? account.Nombre : "Usuario privado",
                   Rol = account.Rol,
                   Insignia = account.Insignia,
                   Estado = account.Estado,
                   Usuario = account.Usuario,
                   Visibilidad = account.Visibilidad,
                   Birthday = account.Visibilidad == AccountVisibility.Visible || account.OrganizationAccess != null && account.OrganizationAccess.Organization.ID == contextOrgID || account.ID == contextUserID ? account.Birthday : new(),
                   Genero = account.Visibilidad == AccountVisibility.Visible || account.OrganizationAccess != null && account.OrganizationAccess.Organization.ID == contextOrgID || account.ID == contextUserID ? account.Genero : Genders.Undefined,
                   Creación = account.Visibilidad == AccountVisibility.Visible || account.OrganizationAccess != null && account.OrganizationAccess.Organization.ID == contextOrgID || account.ID == contextUserID ? account.Creación : default,
                   Perfil = account.Visibilidad == AccountVisibility.Visible || account.OrganizationAccess != null && account.OrganizationAccess.Organization.ID == contextOrgID || account.ID == contextUserID ? account.Perfil : profile,
                   OrganizationAccess = includeOrg && account.OrganizationAccess != null && (account.OrganizationAccess.Organization.ID == contextOrgID || account.ID == contextUserID) ? new OrganizationAccessModel()
                   {
                       ID = account.OrganizationAccess.ID,
                       Rol = account.OrganizationAccess.Rol
                   } : null
               };
    }



    /// <summary>
    /// Armar el modelo final
    /// </summary>
    /// <param name="query">Query base</param>
    /// <param name="contextUserID">Usuario de contexto</param>
    /// <param name="contextOrgID">Organización de contexto</param>
    private static IQueryable<AccountModel> BuildModel(IQueryable<AccountModel> query, bool includeOrg)
    {

        byte[] profile =
        {
        };
        try
        {
            // Imagen genérica
            profile = File.ReadAllBytes("wwwroot/user.png");
        }
        catch { }

        return from account in query
               select new AccountModel
               {
                   ID = account.ID,
                   Nombre = account.Nombre,
                   Rol = account.Rol,
                   Insignia = account.Insignia,
                   Estado = account.Estado,
                   Usuario = account.Usuario,
                   Visibilidad = account.Visibilidad,
                   Birthday = account.Birthday,
                   Genero = account.Genero,
                   Contraseña = account.Contraseña,
                   Creación = account.Creación,
                   Perfil = account.Perfil,
                   OrganizationAccess = includeOrg && account.OrganizationAccess != null ? new OrganizationAccessModel()
                   {
                       ID = account.OrganizationAccess.ID,
                       Rol = account.OrganizationAccess.Rol,
                       Organization = new()
                       {
                           ID = account.OrganizationAccess.Organization.ID,
                           Name = account.OrganizationAccess.Organization.Name,
                           Domain = account.OrganizationAccess.Organization.Domain
                       }
                   } : null
               };
    }


}