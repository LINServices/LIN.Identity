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
        IQueryable<AccountModel> accounts = from account in context.DataBase.Accounts
                                            select account;

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
        IQueryable<AccountModel> accounts = from account in GetAccounts(context)
                                            where account.Estado == AccountStatus.Normal
                                            select account;

        // Retorno
        return accounts;

    }









    public static IQueryable<AccountModel> GetAccounts(int id, FilterModels.Account filters, Conexión context)
    {

        // Query general
        IQueryable<AccountModel> accounts;

        if (filters.FindOn == FilterModels.FindOn.StableAccounts)
            accounts = from account in GetValidAccounts(context)
                       where account.ID == id
                       select account;
        else
            accounts = from account in GetAccounts(context)
                       where account.ID == id
                       select account;

        // Armar el modelo
        accounts = BuildModel(accounts, filters);

        // Retorno
        return accounts;

    }


    public static IQueryable<AccountModel> GetAccounts(string user, FilterModels.Account filters, Conexión context)
    {

        // Query general
        IQueryable<AccountModel> accounts;

        if (filters.FindOn == FilterModels.FindOn.StableAccounts)
            accounts = from account in GetValidAccounts(context)
                       where account.Identity.Unique == user
                       select account;
        else
            accounts = from account in GetAccounts(context)
                       where account.Identity.Unique == user
                       select account;

        // Armar el modelo
        accounts = BuildModel(accounts, filters);

        // Retorno
        return accounts;

    }


    public static IQueryable<AccountModel> GetAccounts(IEnumerable<int> ids, FilterModels.Account filters, Conexión context)
    {

        // Query general
        IQueryable<AccountModel> accounts = from account in GetValidAccounts(context)
                                            where ids.Contains(account.ID)
                                            select account;

        // Armar el modelo
        accounts = BuildModel(accounts, filters);

        // Retorno
        return accounts;

    }


    public static IQueryable<AccountModel> Search(string pattern, FilterModels.Account filters, Conexión context)
    {

        // Query general
        IQueryable<AccountModel> accounts = from account in GetValidAccounts(context)
                                            where account.Identity.Unique.Contains(pattern)
                                            select account;

        // Armar el modelo
        accounts = BuildModel(accounts, filters);

        // Retorno
        return accounts;

    }




    public static IQueryable<DirectoryModel> GetDirectory(int id, Conexión context)
    {

        // Query general
        IQueryable<DirectoryModel> accounts;


        var directories = from directory in context.DataBase.Directories
                          where directory.ID == id
                          select directory;



        // Armar el modelo
        accounts = BuildModel(directories);

        // Retorno
        return accounts;

    }







    /// <summary>
    /// Construir la consulta
    /// </summary>
    /// <param name="query">Query base</param>
    /// <param name="filters">Filtros</param>
    private static IQueryable<AccountModel> BuildModel(IQueryable<AccountModel> query, FilterModels.Account filters)
    {

        byte[] profile =
        {
        };
        try
        {
            profile = File.ReadAllBytes("wwwroot/user.png");
        }
        catch { }

        var queryFinal = from account in query
                         select new AccountModel
                         {
                             ID = account.ID,
                             Rol = account.Rol,
                             IdentityId = account.Identity.Id,
                             Insignia = account.Insignia,
                             Estado = account.Estado,
                             Contraseña = filters.SensibleInfo ? account.Contraseña : "",
                             Visibilidad = account.Visibilidad,
                             Identity = new()
                             {
                                 Id = account.Identity.Id,
                                 Unique = account.Identity.Unique,
                                 Type = account.Identity.Type,
                             },

                             // Nombre.
                             Nombre = account.Visibilidad == AccountVisibility.Visible
                                      || (account.OrganizationAccess != null
                                      && account.OrganizationAccess.Organization.ID == filters.ContextOrg)
                                      || account.ID == filters.ContextUser
                                      || filters.IsAdmin
                                 ? account.Nombre
                                 : "Usuario privado",

                             // Cumpleaños.
                             Birthday = account.Visibilidad == AccountVisibility.Visible
                                        || (account.OrganizationAccess != null
                                        && account.OrganizationAccess.Organization.ID == filters.ContextOrg)
                                        || account.ID == filters.ContextUser
                                        || filters.IsAdmin
                                 ? account.Birthday
                                 : default,

                             // Creación.
                             Creación = account.Visibilidad == AccountVisibility.Visible
                                        || (account.OrganizationAccess != null
                                        && account.OrganizationAccess.Organization.ID == filters.ContextOrg)
                                        || account.ID == filters.ContextUser
                                        || filters.IsAdmin
                                 ? account.Creación
                                 : default,

                             // Perfil.
                             Perfil = account.Visibilidad == AccountVisibility.Visible
                                        || (account.OrganizationAccess != null
                                        && account.OrganizationAccess.Organization.ID == filters.ContextOrg)
                                        || account.ID == filters.ContextUser
                                        || filters.IsAdmin
                                 ? account.Perfil
                                 : profile,

                             // Organización.
                             OrganizationAccess = account.OrganizationAccess != null
                                                  && filters.IncludeOrg != FilterModels.IncludeOrg.None
                                                  && (account.Visibilidad == AccountVisibility.Visible
                                                  && filters.IncludeOrg == FilterModels.IncludeOrg.Include
                                                  || account.OrganizationAccess.Organization.ID == filters.ContextOrg
                                                  || account.ID == filters.ContextUser
                                                  || filters.IsAdmin)
                                 ?
                                 new OrganizationAccessModel()
                                 {
                                     Rol = account.OrganizationAccess.Rol,
                                     Organization = filters.OrgLevel == FilterModels.IncludeOrgLevel.Advance ? new()
                                     {
                                         ID = account.OrganizationAccess.Organization.ID,
                                         Domain = !account.OrganizationAccess.Organization.IsPublic && !filters.IsAdmin && filters.IncludeOrg == FilterModels.IncludeOrg.IncludeIf && filters.ContextOrg != account.OrganizationAccess.Organization.ID ? ""
                                             : account.OrganizationAccess.Organization.Domain,
                                         Name = !account.OrganizationAccess.Organization.IsPublic && !filters.IsAdmin && filters.IncludeOrg == FilterModels.IncludeOrg.IncludeIf && filters.ContextOrg != account.OrganizationAccess.Organization.ID
                                             ? "Organización privada" : account.OrganizationAccess.Organization.Name
                                     } : new(),
                                     Member = null!,
                                 }
                                 : null
                         };

        return queryFinal;

    }



    /// <summary>
    /// Construir la consulta
    /// </summary>
    /// <param name="query">Query base</param>
    /// <param name="filters">Filtros</param>
    private static IQueryable<DirectoryModel> BuildModel(IQueryable<DirectoryModel> query)
    {

        return from d in query
               select new DirectoryModel
               {
                   Creación = d.Creación,
                   ID = d.ID,
                   Identity = new()
                   {
                       Id = d.Identity.Id,
                       Type = d.Identity.Type,
                       Unique = d.Identity.Unique
                   },
                   IdentityId = d.Identity.Id,
                   Nombre = d.Nombre
               };
    }


}