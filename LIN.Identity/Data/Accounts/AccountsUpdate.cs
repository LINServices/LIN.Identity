﻿namespace LIN.Identity.Data;


internal static partial class Accounts
{


    #region Abstracciones


    /// <summary>
    /// Elimina una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    public static async Task<ResponseBase> Delete(int id)
    {

        // Obtiene la conexión
        var (context, connectionKey) = Conexión.GetOneConnection();
        var res = await Delete(id, context);
        context.CloseActions(connectionKey);
        return res;
    }



    /// <summary>
    /// Actualiza la información de una cuenta
    /// </summary>
    /// <param name="modelo">Modelo nuevo de la cuenta</param>
    public static async Task<ResponseBase> Update(AccountModel modelo)
    {

        // Obtiene la conexión
        var (context, connectionKey) = Conexión.GetOneConnection();
        var res = await Update(modelo, context);
        context.CloseActions(connectionKey);
        return res;
    }



  

    /// <summary>
    /// Actualiza el estado de un usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="status">Nuevo estado</param>
    public static async Task<ResponseBase> Update(int id, AccountStatus status)
    {

        var (context, key) = Conexión.GetOneConnection();

        var res = await Update(id, status, context);
        context.CloseActions(key);
        return res;

    }



    /// <summary>
    /// Actualiza el genero de un usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="gender">Nuevo genero</param>
    public static async Task<ResponseBase> Update(int id, Genders gender)
    {

        var (context, key) = Conexión.GetOneConnection();

        var res = await Update(id, gender, context);
        context.CloseActions(key);
        return res;

    }



    /// <summary>
    /// Actualiza la visibilidad de un usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="visibility">Nueva visibilidad</param>
    public static async Task<ResponseBase> Update(int id, AccountVisibility visibility)
    {

        var (context, key) = Conexión.GetOneConnection();

        var res = await Update(id, visibility, context);
        context.CloseActions(key);
        return res;

    }



    public static async Task<ResponseBase> Update(int id, string password)
    {

        var (context, key) = Conexión.GetOneConnection();

        var res = await Update(id, password, context);
        context.CloseActions(key);
        return res;

    }


    #endregion


    /// <summary>
    /// Elimina una cuenta.
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="context">Contexto de conexión</param>
    public static async Task<ResponseBase> Delete(int id, Conexión context)
    {

        // Ejecución
        try
        {

            // Obtiene el usuario
            var user = await context.DataBase.Accounts.FindAsync(id);

            // Si no existe
            if (user == null)
                return new(Responses.Success);

            // Cambia el estado
            user.Estado = AccountStatus.Deleted;
            context.DataBase.SaveChanges();

            // Retorna
            return new(Responses.Success);
        }
        catch
        {
        }

        return new();
    }



    /// <summary>
    /// Actualiza la información de una cuenta
    /// ** No actualiza datos sensibles
    /// </summary>
    /// <param name="modelo">Modelo nuevo de la cuenta</param>
    /// <param name="context">Contexto de conexión</param>
    public static async Task<ResponseBase> Update(AccountModel modelo, Conexión context)
    {

        // Ejecución
        try
        {
            var user = await context.DataBase.Accounts.FindAsync(modelo.ID);

            // Si el usuario no se encontró
            if (user == null || user.Estado != AccountStatus.Normal)
            {
                return new(Responses.NotExistAccount);
            }

            // Nuevos datos
            user.Perfil = modelo.Perfil;
            user.Nombre = modelo.Nombre;

            context.DataBase.SaveChanges();
            return new(Responses.Success);
        }
        catch
        {
        }

        return new();
    }




    /// <summary>
    /// Actualiza la organización de una cuenta
    /// </summary>
    /// <param name="newData">organización</param>
    /// <param name="context">Contexto de conexión con la BD</param>
    public static async Task<ResponseBase> Update(OrganizationModel newData, int id, Conexión context)
    {

        // Encontrar el usuario
        var usuario = await (from U in context.DataBase.OrganizationAccess
                             where U.Member.ID == id
                             select U).FirstOrDefaultAsync();


        var org = await (from U in context.DataBase.Organizations
                         where U.ID == newData.ID
                         select U
            ).FirstOrDefaultAsync();



        // Si el usuario no existe
        if (usuario == null || org == null)
        {
            return new(Responses.NotExistAccount);
        }

        // Cambiar Contraseña
        usuario.Organization = org;

        context.DataBase.SaveChanges();
        return new(Responses.Success);

    }



    /// <summary>
    /// Actualiza el estado
    /// </summary>
    /// <param name="user">ID</param>
    /// <param name="status">Nuevo estado</param>
    /// <param name="context">Contexto de conexión con la BD</param>
    public static async Task<ResponseBase> Update(int user, AccountStatus status, Conexión context)
    {

        // Encontrar el usuario
        var usuario = await (from U in context.DataBase.Accounts
                             where U.ID == user
                             select U).FirstOrDefaultAsync();

        // Si el usuario no existe
        if (usuario == null)
        {
            return new(Responses.NotExistAccount);
        }

        // Cambiar Contraseña
        usuario.Estado = status;

        context.DataBase.SaveChanges();
        return new(Responses.Success);

    }



    /// <summary>
    /// Actualiza el genero
    /// </summary>
    /// <param name="user">ID</param>
    /// <param name="genero">Nuevo genero</param>
    /// <param name="context">Contexto de conexión con la BD</param>
    public static async Task<ResponseBase> Update(int user, Genders genero, Conexión context)
    {

        // Encontrar el usuario
        var usuario = await (from U in context.DataBase.Accounts
                             where U.ID == user
                             select U).FirstOrDefaultAsync();

        // Si el usuario no existe
        if (usuario == null)
        {
            return new(Responses.NotExistAccount);
        }

        // Cambiar Contraseña
       // usuario.Genero = genero;

        context.DataBase.SaveChanges();
        return new(Responses.Success);

    }



    /// <summary>
    /// Actualiza la visibilidad
    /// </summary>
    /// <param name="user">ID</param>
    /// <param name="visibility">Nueva visibilidad</param>
    /// <param name="context">Contexto de conexión con la BD</param>
    public static async Task<ResponseBase> Update(int user, AccountVisibility visibility, Conexión context)
    {

        // Encontrar el usuario
        var usuario = await (from U in context.DataBase.Accounts
                             where U.ID == user
                             select U).FirstOrDefaultAsync();

        // Si el usuario no existe
        if (usuario == null)
        {
            return new(Responses.NotExistAccount);
        }

        // Cambiar visibilidad
        usuario.Visibilidad = visibility;

        context.DataBase.SaveChanges();
        return new(Responses.Success);

    }




    /// <summary>
    /// Actualiza la contraseña
    /// </summary>
    /// <param name="user">ID</param>
    /// <param name="password">Nueva contraseña</param>
    /// <param name="context">Contexto de conexión con la BD</param>
    public static async Task<ResponseBase> Update(int user, string password, Conexión context)
    {

        // Encontrar el usuario
        var usuario = await (from U in context.DataBase.Accounts
                             where U.ID == user
                             select U).FirstOrDefaultAsync();

        // Si el usuario no existe
        if (usuario == null)
        {
            return new(Responses.NotExistAccount);
        }

        // Cambiar visibilidad
        usuario.Contraseña = password;

        context.DataBase.SaveChanges();
        return new(Responses.Success);

    }





}