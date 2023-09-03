namespace LIN.Auth.Data.Filters;


public static class Account
{


    /// <summary>
    /// Si el usuario es oculto/privado devuelve datos genéricos
    /// </summary>
    /// <param name="baseQuery">Query base</param>
    public static IQueryable<AccountModel> Filter(IQueryable<AccountModel> baseQuery, bool safe, bool includeOrg, bool privateInfo)
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
            Nombre = T.Nombre,
            Rol = T.Rol,
            Insignia = T.Insignia,
            Estado = T.Estado,
            Usuario = T.Usuario,
            Contraseña = T.Contraseña,
            Visibilidad = T.Visibilidad,
            Genero = (T.Visibilidad == AccountVisibility.Visible || privateInfo) ? T.Genero : Genders.Undefined,
            Creación = (T.Visibilidad == AccountVisibility.Visible || privateInfo) ? T.Creación : default,
            Perfil = (T.Visibilidad == AccountVisibility.Visible || privateInfo) ? T.Perfil : profile,

            OrganizationAccess = (T.OrganizationAccess != null && includeOrg) ? new OrganizationAccessModel()
            {
                ID = T.OrganizationAccess.ID,
                Rol = T.OrganizationAccess.Rol,
                Organization = T.OrganizationAccess.Organization,
            } : null

        });

        return finalQuery;

    }

}