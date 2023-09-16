namespace LIN.Identity.Data.Filters;


public static class Account
{


    /// <summary>
    /// Si el usuario es oculto/privado devuelve datos genéricos
    /// </summary>
    /// <param name="baseQuery">Query base</param>
    public static IQueryable<AccountModel> Filter(IQueryable<AccountModel> baseQuery, bool safe, bool includeOrg, bool privateInfo, bool sensible)
    {
        byte[] profile = { };
        try
        {
            // Imagen genérica
            profile = File.ReadAllBytes("wwwroot/user.png");
        }
        catch { }

        // Filtro seguro
        if (safe)
            baseQuery = baseQuery.Where(T => T.Estado == AccountStatus.Normal);

        // Generación de la consulta
        var finalQuery = baseQuery.Select(T => new AccountModel
        {
            ID = T.ID,
            Nombre = T.Visibilidad == AccountVisibility.Visible || privateInfo ? T.Nombre : "Usuario privado",
            Rol = T.Rol,
            Insignia = T.Insignia,
            Estado = T.Estado,
            Usuario = T.Usuario,
            Contraseña = sensible ? T.Contraseña : "",
            Visibilidad = T.Visibilidad,
            Genero = T.Visibilidad == AccountVisibility.Visible || privateInfo ? T.Genero : Genders.Undefined,
            Creación = T.Visibilidad == AccountVisibility.Visible || privateInfo ? T.Creación : default,
            Perfil = T.Visibilidad == AccountVisibility.Visible || privateInfo ? T.Perfil : profile,

            OrganizationAccess = T.OrganizationAccess != null && includeOrg ? new OrganizationAccessModel()
            {
                ID = T.OrganizationAccess.ID,
                Rol = T.OrganizationAccess.Rol,
                Organization = T.OrganizationAccess.Organization,
            } : null

        });

        return finalQuery;

    }



    /// <summary>
    /// Si el usuario es oculto/privado devuelve datos genéricos
    /// </summary>
    /// <param name="baseQuery">Query base</param>
    public static IQueryable<AccountModel> Filter(IQueryable<AccountModel> baseQuery, int orgID)
    {
        byte[] profile = { };
        try
        {
            // Imagen genérica
            profile = File.ReadAllBytes("wwwroot/user.png");
        }
        catch { }

        // Filtro seguro
        baseQuery = baseQuery.Where(T => T.Estado == AccountStatus.Normal);

        // Generación de la consulta
        var finalQuery = baseQuery.Select(T => new AccountModel
        {
            ID = T.ID,
            Nombre = T.Visibilidad == AccountVisibility.Visible || (T.OrganizationAccess.Organization.ID == orgID) ? T.Nombre : "Usuario privado",
            Rol = T.Rol,
            Insignia = T.Insignia,
            Estado = T.Estado,
            Usuario = T.Usuario,
            Visibilidad = T.Visibilidad,
            Genero = T.Visibilidad == AccountVisibility.Visible || (T.OrganizationAccess.Organization.ID == orgID) ? T.Genero : Genders.Undefined,
            Creación = T.Visibilidad == AccountVisibility.Visible || (T.OrganizationAccess.Organization.ID == orgID) ? T.Creación : default,
            Perfil = T.Visibilidad == AccountVisibility.Visible || (T.OrganizationAccess.Organization.ID == orgID) ? T.Perfil : profile,
            
            OrganizationAccess = (T.OrganizationAccess != null && T.OrganizationAccess.Organization.ID == orgID) ? new OrganizationAccessModel()
            {
                ID = T.OrganizationAccess.ID,
                Rol = T.OrganizationAccess.Rol
            } : new()
        });

        return finalQuery;

    }


}