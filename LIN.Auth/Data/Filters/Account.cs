namespace LIN.Auth.Data.Filters;


public static class Account
{


    /// <summary>
    /// Si el usuario es oculto/privado devuelve datos genéricos
    /// </summary>
    /// <param name="baseQuery">Query base</param>
    public static IQueryable<AccountModel> FilterInfoIf(IQueryable<AccountModel> baseQuery)
    {

        // Imagen genérica
        var profile = File.ReadAllBytes("wwwroot/user.png");

        // Generación de la consulta
        var finalQuery = baseQuery.Select(T => new AccountModel
        {
            ID = T.ID,
            Nombre = T.Nombre,
            Rol = T.Rol,
            Insignia = T.Insignia,
            Estado = T.Estado,
            Usuario = T.Usuario,
            Visibilidad = T.Visibilidad,
            Genero = (T.Visibilidad == AccountVisibility.Visible) ? T.Genero : Genders.Undefined,
            Creación = (T.Visibilidad == AccountVisibility.Visible) ? T.Creación : default,
            Perfil = (T.Visibilidad == AccountVisibility.Visible) ? T.Perfil : profile,
            Organization = T.Organization
        }) ;

        return finalQuery;

    }



}