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
    public static IQueryable<AccountModel> GetStablishAccounts(int userID, int contextUserID, int contextOrgID, Conexión context)
    {

        // Query general
        IQueryable<AccountModel> accounts = from account in GetValidAccounts(context)
                                            where account.ID == userID
                                            select account;

        // Armar el modelo
        accounts = BuildModel(accounts, contextUserID, contextOrgID);

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
    public static IQueryable<AccountModel> GetStablishAccounts(IEnumerable<int> ids, int contextUserID, int contextOrgID, Conexión context)
    {

        // Query general
        IQueryable<AccountModel> accounts = from account in GetValidAccounts(context)
                                            where ids.Contains(account.ID)
                                            select account;

        // Armar el modelo
        accounts = BuildModel(accounts, contextUserID, contextOrgID);

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
    public static IQueryable<AccountModel> Search(string pattern, int contextUserID, int contextOrgID, Conexión context)
    {

        // Query general
        IQueryable<AccountModel> accounts = from account in GetValidAccounts(context)
                                            where account.Usuario.ToLower().Contains(pattern.ToLower())
                                            && account.ID != contextUserID
                                            select account;

        // Armar el modelo
        accounts = BuildModel(accounts, contextUserID, contextOrgID);

        // Retorno
        return accounts;

    }



    /// <summary>
    /// Armar el modelo final
    /// </summary>
    /// <param name="query">Query base</param>
    /// <param name="contextUserID">Usuario de contexto</param>
    /// <param name="contextOrgID">Organización de contexto</param>
    private static IQueryable<AccountModel> BuildModel(IQueryable<AccountModel> query, int contextUserID, int contextOrgID)
    {

        byte[] profile = { };
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
                   Nombre = (account.Visibilidad == AccountVisibility.Visible || (account.OrganizationAccess != null && account.OrganizationAccess.Organization.ID == contextOrgID) || account.ID == contextUserID) ? account.Nombre : "Usuario privado",
                   Rol = account.Rol,
                   Insignia = account.Insignia,
                   Estado = account.Estado,
                   Usuario = account.Usuario,
                   Visibilidad = account.Visibilidad,
                   Birthday = (account.Visibilidad == AccountVisibility.Visible || (account.OrganizationAccess != null && account.OrganizationAccess.Organization.ID == contextOrgID) || account.ID == contextUserID) ? account.Birthday : new(),
                   Genero = (account.Visibilidad == AccountVisibility.Visible || (account.OrganizationAccess != null && account.OrganizationAccess.Organization.ID == contextOrgID) || account.ID == contextUserID) ? account.Genero : Genders.Undefined,
                   Creación = (account.Visibilidad == AccountVisibility.Visible || (account.OrganizationAccess != null && account.OrganizationAccess.Organization.ID == contextOrgID) || account.ID == contextUserID) ? account.Creación : default,
                   Perfil = (account.Visibilidad == AccountVisibility.Visible || (account.OrganizationAccess != null && account.OrganizationAccess.Organization.ID == contextOrgID) || account.ID == contextUserID) ? account.Perfil : profile,
                   OrganizationAccess = (account.OrganizationAccess != null && (account.OrganizationAccess.Organization.ID == contextOrgID || account.ID == contextUserID)) ? new OrganizationAccessModel()
                   {
                       ID = account.OrganizationAccess.ID,
                       Rol = account.OrganizationAccess.Rol,
                   } : new()
               };
    }


}