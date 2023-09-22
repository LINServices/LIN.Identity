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
    /// Solo tiene en cuenta cuentas validas
    /// </summary>
    /// <param name="context">Contexto DB</param>
    public static IQueryable<AccountModel> GetStablishAccounts(int userID, int contextUserID, int contextOrgID, Conexión context)
    {

        byte[] profile = { };
        try
        {
            // Imagen genérica
            profile = File.ReadAllBytes("wwwroot/user.png");
        }
        catch { }

        // Query general
        IQueryable<AccountModel> accounts = from account in GetValidAccounts(context)
                                            where account.ID == userID
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

        var query = accounts.ToQueryString();

        // Retorno
        return accounts;

    }



    /// <summary>
    /// Solo tiene en cuenta cuentas validas
    /// </summary>
    /// <param name="context">Contexto DB</param>
    public static IQueryable<AccountModel> GetStablishAccounts(IEnumerable<int> ids, int contextUserID, int contextOrgID, Conexión context)
    {

        byte[] profile = { };
        try
        {
            // Imagen genérica
            profile = File.ReadAllBytes("wwwroot/user.png");
        }
        catch { }

        // Query general
        IQueryable<AccountModel> accounts = from account in GetValidAccounts(context)
                                            where ids.Contains(account.ID)
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

        // Retorno
        return accounts;

    }


}