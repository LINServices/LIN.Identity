namespace LIN.Identity.Validations;

public static class Roles
{

    /// <summary>
    /// Confirmar si un rol tiene permisos de ver el directorio.
    /// </summary>
    /// <param name="rol">Rol a confirmar.</param>
    public static bool View(Types.Identity.Enumerations.Roles rol)
    {

        Types.Identity.Enumerations.Roles[] roles =
        [
            Types.Identity.Enumerations.Roles.System,
            Types.Identity.Enumerations.Roles.SuperManager,
            Types.Identity.Enumerations.Roles.Manager,
            Types.Identity.Enumerations.Roles.Operator,
            Types.Identity.Enumerations.Roles.AccountsOperator,
            Types.Identity.Enumerations.Roles.Regular,
            Types.Identity.Enumerations.Roles.Guest,
            Types.Identity.Enumerations.Roles.RoyalGuest
        ];

        return roles.Contains(rol);

    }


    /// <summary>
    /// Confirmar si un rol tiene permisos de ver los integrantes.
    /// </summary>
    /// <param name="rol">Rol a confirmar.</param>
    public static bool ViewMembers(Types.Identity.Enumerations.Roles rol)
    {

        Types.Identity.Enumerations.Roles[] roles =
        [
            Types.Identity.Enumerations.Roles.System,
            Types.Identity.Enumerations.Roles.SuperManager,
            Types.Identity.Enumerations.Roles.Manager,
            Types.Identity.Enumerations.Roles.Operator,
            Types.Identity.Enumerations.Roles.AccountsOperator,
            Types.Identity.Enumerations.Roles.Regular,
            Types.Identity.Enumerations.Roles.Guest,
            Types.Identity.Enumerations.Roles.RoyalGuest
        ];

        return roles.Contains(rol);

    }


    /// <summary>
    /// Confirmar si un rol tiene permisos para alterar los integrantes.
    /// </summary>
    /// <param name="rol">Rol a confirmar.</param>
    public static bool AlterMembers(Types.Identity.Enumerations.Roles rol)
    {

        Types.Identity.Enumerations.Roles[] roles =
        [
            Types.Identity.Enumerations.Roles.System,
            Types.Identity.Enumerations.Roles.SuperManager,
            Types.Identity.Enumerations.Roles.Manager,
            Types.Identity.Enumerations.Roles.Operator,
            Types.Identity.Enumerations.Roles.AccountsOperator,
            Types.Identity.Enumerations.Roles.RoyalGuest
        ];

        return roles.Contains(rol);

    }


    /// <summary>
    /// Confirmar si un rol tiene permisos de saltarse las directivas.
    /// </summary>
    /// <param name="rol">Rol a confirmar.</param>
    public static bool UsePolicy(Types.Identity.Enumerations.Roles rol)
    {

        Types.Identity.Enumerations.Roles[] roles =
        [
            Types.Identity.Enumerations.Roles.System,
            Types.Identity.Enumerations.Roles.Guest,
            Types.Identity.Enumerations.Roles.RoyalGuest
        ];

        return roles.Contains(rol);

    }


    /// <summary>
    /// Confirmar si un rol tiene permisos de crear directivas.
    /// </summary>
    /// <param name="rol">Rol a confirmar.</param>
    public static bool CreatePolicy(Types.Identity.Enumerations.Roles rol)
    {

        Types.Identity.Enumerations.Roles[] roles =
        [
            Types.Identity.Enumerations.Roles.System,
            Types.Identity.Enumerations.Roles.SuperManager,
            Types.Identity.Enumerations.Roles.Manager,
            Types.Identity.Enumerations.Roles.Operator,
            Types.Identity.Enumerations.Roles.RoyalGuest
        ];

        return roles.Contains(rol);

    }



    /// <summary>
    /// Confirmar si un rol tiene permisos de alterar datos como nombres de la organización etc...
    /// </summary>
    /// <param name="rol">Rol a confirmar.</param>
    public static bool DataAlter(Types.Identity.Enumerations.Roles rol)
    {

        Types.Identity.Enumerations.Roles[] roles =
        [
            Types.Identity.Enumerations.Roles.System,
            Types.Identity.Enumerations.Roles.SuperManager,
            Types.Identity.Enumerations.Roles.Manager,
        ];

        return roles.Contains(rol);

    }


    /// <summary>
    /// Confirmar si un rol tiene permisos de saltarse las directivas.
    /// </summary>
    /// <param name="rol">Rol a confirmar.</param>
    public static bool ViewPolicy(Types.Identity.Enumerations.Roles rol)
    {

        Types.Identity.Enumerations.Roles[] roles =
        [
            Types.Identity.Enumerations.Roles.System,
            Types.Identity.Enumerations.Roles.SuperManager,
            Types.Identity.Enumerations.Roles.Manager,
            Types.Identity.Enumerations.Roles.Operator,
            Types.Identity.Enumerations.Roles.AccountsOperator,
            Types.Identity.Enumerations.Roles.Regular,
            Types.Identity.Enumerations.Roles.RoyalGuest
        ];

        return roles.Contains(rol);

    }







    /// <summary>
    /// Confirmar si un rol tiene permisos de ver el directorio.
    /// </summary>
    /// <param name="rol">Rol a confirmar.</param>
    public static bool View(IEnumerable<Types.Identity.Enumerations.Roles> roles)
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
    public static bool ViewMembers(IEnumerable<Types.Identity.Enumerations.Roles> roles)
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
    public static bool AlterMembers(IEnumerable<Types.Identity.Enumerations.Roles> roles)
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
    public static bool UsePolicy(IEnumerable<Types.Identity.Enumerations.Roles> roles)
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
    public static bool CreatePolicy(IEnumerable<Types.Identity.Enumerations.Roles> roles)
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
    public static bool DataAlter(IEnumerable<Types.Identity.Enumerations.Roles> roles)
    {
        // Recorrer roles.
        foreach (var rol in roles)
            if (DataAlter(rol))
                return true;

        // No tiene permisos.
        return false;

    }



    public static bool ViewPolicy(IEnumerable<Types.Identity.Enumerations.Roles> roles)
    {
        // Recorrer roles.
        foreach (var rol in roles)
            if (ViewPolicy(rol))
                return true;

        // No tiene permisos.
        return false;

    }

}