namespace LIN.Identity.Validations;

public static class Roles
{

    /// <summary>
    /// Confirmar si un rol tiene permisos de ver el directorio.
    /// </summary>
    /// <param name="rol">Rol a confirmar.</param>
    public static bool View(DirectoryRoles rol)
    {

        DirectoryRoles[] roles =
        [
            DirectoryRoles.System,
            DirectoryRoles.SuperManager,
            DirectoryRoles.Manager,
            DirectoryRoles.Operator,
            DirectoryRoles.AccountsOperator,
            DirectoryRoles.Regular,
            DirectoryRoles.Guest,
            DirectoryRoles.RoyalGuest
        ];

        return roles.Contains(rol);

    }



    /// <summary>
    /// Confirmar si un rol tiene permisos de ver los integrantes.
    /// </summary>
    /// <param name="rol">Rol a confirmar.</param>
    public static bool ViewMembers(DirectoryRoles rol)
    {

        DirectoryRoles[] roles =
        [
            DirectoryRoles.System,
            DirectoryRoles.SuperManager,
            DirectoryRoles.Manager,
            DirectoryRoles.Operator,
            DirectoryRoles.AccountsOperator,
            DirectoryRoles.Regular,
            DirectoryRoles.Guest,
            DirectoryRoles.RoyalGuest
        ];

        return roles.Contains(rol);

    }



    /// <summary>
    /// Confirmar si un rol tiene permisos para alterar los integrantes.
    /// </summary>
    /// <param name="rol">Rol a confirmar.</param>
    public static bool AlterMembers(DirectoryRoles rol)
    {

        DirectoryRoles[] roles =
        [
            DirectoryRoles.System,
            DirectoryRoles.SuperManager,
            DirectoryRoles.Manager,
            DirectoryRoles.Operator,
            DirectoryRoles.AccountsOperator,
            DirectoryRoles.RoyalGuest
        ];

        return roles.Contains(rol);

    }


    /// <summary>
    /// Confirmar si un rol tiene permisos de saltarse las directivas.
    /// </summary>
    /// <param name="rol">Rol a confirmar.</param>
    public static bool UsePolicy(DirectoryRoles rol)
    {

        DirectoryRoles[] roles =
        [
            DirectoryRoles.System,
            DirectoryRoles.Guest,
            DirectoryRoles.RoyalGuest
        ];

        return roles.Contains(rol);

    }


    /// <summary>
    /// Confirmar si un rol tiene permisos de crear directivas.
    /// </summary>
    /// <param name="rol">Rol a confirmar.</param>
    public static bool CreatePolicy(DirectoryRoles rol)
    {

        DirectoryRoles[] roles =
        [
            DirectoryRoles.System,
            DirectoryRoles.SuperManager,
            DirectoryRoles.Manager,
            DirectoryRoles.Operator,
            DirectoryRoles.RoyalGuest
        ];

        return roles.Contains(rol);

    }



    /// <summary>
    /// Confirmar si un rol tiene permisos de alterar datos como nombres de la organización etc...
    /// </summary>
    /// <param name="rol">Rol a confirmar.</param>
    public static bool DataAlter(DirectoryRoles rol)
    {

        DirectoryRoles[] roles =
        [
            DirectoryRoles.System,
            DirectoryRoles.SuperManager,
            DirectoryRoles.Manager,
        ];

        return roles.Contains(rol);

    }


    /// <summary>
    /// Confirmar si un rol tiene permisos de saltarse las directivas.
    /// </summary>
    /// <param name="rol">Rol a confirmar.</param>
    public static bool ViewPolicy(DirectoryRoles rol)
    {

        DirectoryRoles[] roles =
        [
            DirectoryRoles.System,
            DirectoryRoles.SuperManager,
            DirectoryRoles.Manager,
            DirectoryRoles.Operator,
            DirectoryRoles.AccountsOperator,
            DirectoryRoles.Regular,
            DirectoryRoles.RoyalGuest
        ];

        return roles.Contains(rol);

    }







    /// <summary>
    /// Confirmar si un rol tiene permisos de ver el directorio.
    /// </summary>
    /// <param name="rol">Rol a confirmar.</param>
    public static bool View(IEnumerable<DirectoryRoles> roles)
    {

        // Recorrer roles.
        foreach(var rol in roles)
            if (View(rol))
                return true;

        // No tiene permisos.
        return false;
    }



    /// <summary>
    /// Confirmar si un rol tiene permisos de ver los integrantes.
    /// </summary>
    /// <param name="rol">Rol a confirmar.</param>
    public static bool ViewMembers(IEnumerable<DirectoryRoles> roles)
    {

        // Recorrer roles.
        foreach (var rol in roles)
            if (ViewMembers(rol))
                return true;

        // No tiene permisos.
        return false;
    }



    /// <summary>
    /// Confirmar si un rol tiene permisos para alterar los integrantes.
    /// </summary>
    /// <param name="rol">Rol a confirmar.</param>
    public static bool AlterMembers(IEnumerable<DirectoryRoles> roles)
    {
        // Recorrer roles.
        foreach (var rol in roles)
            if (AlterMembers(rol))
                return true;

        // No tiene permisos.
        return false;

    }


    /// <summary>
    /// Confirmar si un rol tiene permisos de saltarse las directivas.
    /// </summary>
    /// <param name="rol">Rol a confirmar.</param>
    public static bool UsePolicy(IEnumerable<DirectoryRoles> roles)
    {

        // Recorrer roles.
        foreach (var rol in roles)
            if (UsePolicy(rol))
                return true;

        // No tiene permisos.
        return false;

    }


    /// <summary>
    /// Confirmar si un rol tiene permisos de crear directivas.
    /// </summary>
    /// <param name="rol">Rol a confirmar.</param>
    public static bool CreatePolicy(IEnumerable<DirectoryRoles> roles)
    {

        // Recorrer roles.
        foreach (var rol in roles)
            if (CreatePolicy(rol))
                return true;

        // No tiene permisos.
        return false;

    }



    /// <summary>
    /// Confirmar si un rol tiene permisos de alterar datos como nombres de la organización etc...
    /// </summary>
    /// <param name="rol">Rol a confirmar.</param>
    public static bool DataAlter(IEnumerable<DirectoryRoles> roles)
    {
        // Recorrer roles.
        foreach (var rol in roles)
            if (DataAlter(rol))
                return true;

        // No tiene permisos.
        return false;

    }



    public static bool ViewPolicy(IEnumerable<DirectoryRoles> roles)
    {
        // Recorrer roles.
        foreach (var rol in roles)
            if (ViewPolicy(rol))
                return true;

        // No tiene permisos.
        return false;

    }

}