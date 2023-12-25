namespace LIN.Identity.Queries;


public class Identities
{



    /// <summary>
    /// Consulta sobre todas lac cuentas.
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
    /// Consulta sobre las cuentas validas.
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









    public static IQueryable<AccountModel> GetAccounts(int id, Models.Account filters, Conexión context)
    {

        // Query general
        IQueryable<AccountModel> accounts;

        if (filters.FindOn == Models.FindOn.StableAccounts)
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

    public static IQueryable<AccountModel> GetAccountsByIdentity(int id, Models.Account filters, Conexión context)
    {

        // Query general
        IQueryable<AccountModel> accounts;

        if (filters.FindOn == Models.FindOn.StableAccounts)
            accounts = from account in GetValidAccounts(context)
                       where account.Identity.Id == id
                       select account;
        else
            accounts = from account in GetAccounts(context)
                       where account.Identity.Id == id
                       select account;

        // Armar el modelo
        accounts = BuildModel(accounts, filters);

        // Retorno
        return accounts;

    }


    public static IQueryable<AccountModel> GetAccounts(string user, Models.Account filters, Conexión context)
    {

        // Query general
        IQueryable<AccountModel> accounts;

        if (filters.FindOn == Models.FindOn.StableAccounts)
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


    public static IQueryable<AccountModel> GetAccounts(IEnumerable<int> ids, Models.Account filters, Conexión context)
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


    public static IQueryable<AccountModel> Search(string pattern, Models.Account filters, Conexión context)
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




    public static IQueryable<DirectoryMember> GetDirectory(int id, int identityContext, Conexión context)
    {

        // Query general
        IQueryable<DirectoryMember> accounts;


        var directory = from dm in context.DataBase.DirectoryMembers
                        where dm.Directory.ID == id
                        select dm;

        if (identityContext > 0)
            directory = directory.Where(t => t.IdentityId == identityContext);

        // Armar el modelo
        accounts = BuildModel(directory);

        // Retorno
        return accounts;

    }


    public static IQueryable<DirectoryMember> GetDirectoryByIdentity(int id, Conexión context)
    {

        // Query general
        IQueryable<DirectoryMember> accounts;


        var directory = from dm in context.DataBase.DirectoryMembers
                        where dm.Directory.Identity.Id == id
                        select dm;

        // Armar el modelo
        accounts = BuildModel(directory);

        // Retorno
        return accounts;

    }








    /// <summary>
    /// Construir la consulta
    /// </summary>
    /// <param name="query">Query base</param>
    /// <param name="filters">Filtros</param>
    private static IQueryable<AccountModel> BuildModel(IQueryable<AccountModel> query, Models.Account filters)
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
                                      || account.ID == filters.ContextAccount
                                      || filters.IsAdmin
                                 ? account.Nombre
                                 : "Usuario privado",

                             // Cumpleaños.
                             Birthday = account.Visibilidad == AccountVisibility.Visible
                                        || account.ID == filters.ContextAccount
                                        || filters.IsAdmin
                                 ? account.Birthday
                                 : default,

                             // Creación.
                             Creación = account.Visibilidad == AccountVisibility.Visible
                                        || account.ID == filters.ContextAccount
                                        || filters.IsAdmin
                                 ? account.Creación
                                 : default,

                             // Perfil.
                             Perfil = account.Visibilidad == AccountVisibility.Visible
                                        || account.ID == filters.ContextAccount
                                        || filters.IsAdmin
                                 ? account.Perfil
                                 : profile,
        
                         };

        return queryFinal;

    }



    /// <summary>
    /// Construir la consulta
    /// </summary>
    /// <param name="query">Query base</param>
    /// <param name="filters">Filtros</param>
    private static IQueryable<DirectoryMember> BuildModel(IQueryable<DirectoryMember> query)
    {

        return from d in query
               select new DirectoryMember
               {
                   IdentityId = d.IdentityId,
                   Directory = new()
                   {
                       Nombre = d.Directory.Nombre,
                       Creación = d.Directory.Creación,
                       ID = d.Directory.ID,
                       Identity = new()
                       {
                           Id = d.Directory.Identity.Id,
                           Type = d.Directory.Identity.Type,
                           Unique = d.Directory.Identity.Unique
                       },
                       IdentityId = d.Directory.Identity.Id
                   }
               };
    }


}