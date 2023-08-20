namespace LIN.Auth.Data;


public static class Accounts
{


    #region Abstracciones



    /// <summary>
    /// Crea un nuevo usuario
    /// </summary>
    /// <param name="data">Modelo del usuario</param>
    public async static Task<ReadOneResponse<AccountModel>> Create(AccountModel data)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await Create(data, context);
        context.CloseActions(connectionKey);
        return res;

    }



    /// <summary>
    /// Obtiene un usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    public async static Task<ReadOneResponse<AccountModel>> Read(int id, bool safeFilter, bool privateInfo = true, bool includeOrg = false)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await Read(id, safeFilter, privateInfo, includeOrg, context);
        context.CloseActions(connectionKey);
        return res;

    }



    /// <summary>
    /// Obtiene un usuario
    /// </summary>
    /// <param name="user">Usuario de la cuenta</param>
    public async static Task<ReadOneResponse<AccountModel>> Read(string user, bool safeFilter, bool privateInfo = true, bool includeOrg = false)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await Read(user, safeFilter, privateInfo, includeOrg, context);
        context.CloseActions(connectionKey);
        return res;
    }



    /// <summary>
    /// Obtiene los primeros 10 usuarios que coincidan con el patron
    /// </summary>
    /// <param name="pattern">Patron a buscar</param>
    /// <param name="id">ID de la cuenta</param>
    public async static Task<ReadAllResponse<AccountModel>> SearchByPattern(string pattern, int id)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await SearchByPattern(pattern, id, context);
        context.CloseActions(connectionKey);
        return res;
    }




    /// <summary>
    /// Obtiene la lista de usuarios correspondiente a los ids
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    public async static Task<ReadAllResponse<AccountModel>> FindAll(List<int> ids)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await FindAll(ids, context);
        context.CloseActions(connectionKey);
        return res;
    }



    /// <summary>
    /// Obtiene los primeros 5 usuarios que coincidan con el patron (ADMIN)
    /// </summary>
    /// <param name="pattern">Patron a buscar</param>
    public async static Task<ReadAllResponse<AccountModel>> GetAll(string pattern)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await GetAll(pattern, context);
        context.CloseActions(connectionKey);
        return res;
    }



    /// <summary>
    /// Elimina una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    public async static Task<ResponseBase> Delete(int id)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();
        var res = await Delete(id, context);
        context.CloseActions(connectionKey);
        return res;
    }



    /// <summary>
    /// Actualiza la información de una cuenta
    /// </summary>
    /// <param name="modelo">Modelo nuevo de la cuenta</param>
    public async static Task<ResponseBase> Update(AccountModel modelo)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();
        var res = await Update(modelo, context);
        context.CloseActions(connectionKey);
        return res;
    }



    /// <summary>
    /// Actualiza las credenciales (Contraseña de un usuario)
    /// </summary>
    /// <param name="newData">Nuevas credenciales</param>
    public async static Task<ResponseBase> UpdatePassword(UpdatePasswordModel newData)
    {

        var (context, key) = Conexión.GetOneConnection();

        var res = await UpdatePassword(newData, context);
        context.CloseActions(key);
        return res;

    }



    /// <summary>
    /// Actualiza el estado de un usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="status">Nuevo estado</param>
    public async static Task<ResponseBase> UpdateState(int id, AccountStatus status)
    {

        var (context, key) = Conexión.GetOneConnection();

        var res = await UpdateState(id, status, context);
        context.CloseActions(key);
        return res;

    }



    /// <summary>
    /// Actualiza el genero de un usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="gender">Nuevo genero</param>
    public async static Task<ResponseBase> UpdateGender(int id, Genders gender)
    {

        var (context, key) = Conexión.GetOneConnection();

        var res = await UpdateGender(id, gender, context);
        context.CloseActions(key);
        return res;

    }



    /// <summary>
    /// Actualiza la visibilidad de un usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="visibility">Nueva visibilidad</param>
    public async static Task<ResponseBase> UpdateVisibility(int id, AccountVisibility visibility)
    {

        var (context, key) = Conexión.GetOneConnection();

        var res = await UpdateVisibility(id, visibility, context);
        context.CloseActions(key);
        return res;

    }




    #endregion



    /// <summary>
    /// Crea una cuenta
    /// </summary>
    /// <param name="data">Modelo</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadOneResponse<AccountModel>> Create(AccountModel data, Conexión context)
    {

        data.ID = 0;

        // Ejecución
        try
        {
            var res = await context.DataBase.Accounts.AddAsync(data);
            context.DataBase.SaveChanges();

            return new(Responses.Success, data);
        }
        catch (Exception ex)
        {
            if ((ex.InnerException?.Message.Contains("Violation of UNIQUE KEY constraint") ?? false) || (ex.InnerException?.Message.Contains("duplicate key") ?? false))
                return new(Responses.ExistAccount);

        }

        return new();
    }



    /// <summary>
    /// Obtiene una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="safeFilter">TRUE para solo obtener usuarios activos</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadOneResponse<AccountModel>> Read(int id, bool safeFilter, bool privateInfo, bool includeOrg, Conexión context)
    {

        // Ejecución
        try
        {

            // Consulta global
            var query = from A in context.DataBase.Accounts
                        where A.ID == id
                        select A;


            if (includeOrg)
            {
                query = query.Include(a => a.Organization).ThenInclude(a => a.AppList).ThenInclude(a => a.App);
            }


            // Filtro seguro
            {
                query = query.Where(T => T.Estado == AccountStatus.Normal);
                query = Filters.Account.Get(query);
            }


            // Si no necesita información privada
            if (!privateInfo)
                query = Filters.Account.FilterInfoIf(query);


            // Obtiene el usuario
            var result = await query.FirstOrDefaultAsync();

            // Si no existe el modelo
            if (result == null)
                return new(Responses.NotExistAccount);

            return new(Responses.Success, result);
        }
        catch
        {
        }

        return new();
    }



    /// <summary>
    /// Obtiene una cuenta
    /// </summary>
    /// <param name="user">Usuario de la cuenta</param>
    /// <param name="safeFilter">TRUE para solo obtener usuarios activos</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadOneResponse<AccountModel>> Read(string user, bool safeFilter, bool privateInfo, bool includeOrg, Conexión context)
    {

        // Ejecución
        try
        {

            // Consulta global
            var query = from A in context.DataBase.Accounts
                        where A.Usuario == user
                        select A;


            if (includeOrg)
            {
                query = query.Include(a => a.Organization).ThenInclude(a => a.AppList).ThenInclude(a => a.App);
            }


            // Filtro seguro
            if (safeFilter)
            {
                query = query.Where(T => T.Estado == AccountStatus.Normal);
                query = Filters.Account.Get(query);
            }

            // Si no necesita información privada
            if (!privateInfo)
                query = Filters.Account.FilterInfoIf(query);

            // Trae la cuenta
            var account = await query.FirstOrDefaultAsync();

            // Si no existe el modelo
            if (account == null)
                return new(Responses.NotExistAccount);

            return new(Responses.Success, account);

        }
        catch
        {
        }

        return new();
    }



    /// <summary>
    /// Obtiene los primeros 10 usuarios que coincidan con el patron
    /// </summary>
    /// <param name="pattern">Patron a buscar</param>
    /// <param name="id">ID de la cuenta (Contexto)</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadAllResponse<AccountModel>> SearchByPattern(string pattern, int id, Conexión context)
    {

        // Ejecución
        try
        {

            // Query
            var query = (from A in context.DataBase.Accounts
                         where A.Usuario.ToLower().Contains(pattern.ToLower())
                         && A.ID != id
                         && A.Visibilidad == AccountVisibility.Visible
                         select A).Take(10);

            // Ejecuta
            var result = await query.Select(a => new AccountModel
            {
                ID = a.ID,
                Nombre = a.Nombre,
                Usuario = a.Usuario,
                Perfil = a.Perfil,
                Genero = a.Genero,
                Insignia = a.Insignia
            }).ToListAsync();


            // Si no existe el modelo
            if (result == null)
                return new(Responses.NotRows);

            return new(Responses.Success, result);
        }
        catch
        {
        }

        return new();
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="pattern">Patron a buscar</param>
    /// <param name="id">ID de la cuenta (Contexto)</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadAllResponse<AccountModel>> FindAll(List<int> ids, Conexión context)
    {

        // Ejecución
        try
        {

            // Query
            var query = (from A in context.DataBase.Accounts
                         where A.Estado == AccountStatus.Normal
                         where ids.Contains(A.ID)
                         select A);

            // Ejecuta
            var result = await Filters.Account.FilterInfoIf(query).ToListAsync();

            // Si no existe el modelo
            if (result == null)
                return new(Responses.NotRows);

            return new(Responses.Success, result);
        }
        catch
        {
        }

        return new();
    }



    /// <summary>
    /// Obtiene las primeros 5 cuentas que coincidan con el patron (Admin)
    /// </summary>
    /// <param name="pattern">Patron a buscar</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadAllResponse<AccountModel>> GetAll(string pattern, Conexión context)
    {

        // Ejecución
        try
        {
            var res = await context.DataBase.Accounts
                .Where(T => T.Usuario.ToLower().Contains(pattern.ToLower()))
                .Take(5)
                .ToListAsync();

            // Si no existe el modelo
            if (res == null)
                return new(Responses.NotRows);

            return new(Responses.Success, res);
        }
        catch
        {
        }

        return new();
    }



    /// <summary>
    /// Elimina una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ResponseBase> Delete(int id, Conexión context)
    {

        // Ejecución
        try
        {
            var user = await context.DataBase.Accounts.FindAsync(id);

            if (user != null)
            {
                user.Estado = AccountStatus.Deleted;
                context.DataBase.SaveChanges();
            }

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
    public async static Task<ResponseBase> Update(AccountModel modelo, Conexión context)
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
            user.Genero = modelo.Genero;

            context.DataBase.SaveChanges();
            return new(Responses.Success);
        }
        catch
        {
        }

        return new();
    }



    /// <summary>
    /// Actualiza la contraseña
    /// </summary>
    /// <param name="newData">Nuevas credenciales</param>
    /// <param name="context">Contexto de conexión con la BD</param>
    public async static Task<ResponseBase> UpdatePassword(UpdatePasswordModel newData, Conexión context)
    {

        // Encontrar el usuario
        var usuario = await (from U in context.DataBase.Accounts
                             where U.ID == newData.Account
                             select U).FirstOrDefaultAsync();

        // Si el usuario no existe
        if (usuario == null)
        {
            return new ResponseBase(Responses.NotExistAccount);
        }

        // Confirmar contraseña
        var newEncrypted = EncryptClass.Encrypt(Conexión.SecreteWord + newData.NewPassword);

        // Cambiar Contraseña
        usuario.Contraseña = newEncrypted;

        context.DataBase.SaveChanges();
        return new(Responses.Success);

    }



    /// <summary>
    /// Actualiza la organización de una cuenta
    /// </summary>
    /// <param name="newData">organización</param>
    /// <param name="context">Contexto de conexión con la BD</param>
    public async static Task<ResponseBase> UpdateOrg(OrganizationModel newData, int id, Conexión context)
    {

        // Encontrar el usuario
        var usuario = await (from U in context.DataBase.Accounts
                             where U.ID == id
                             select U).Include(a => a.Organization).FirstOrDefaultAsync();


        var org = await (from U in context.DataBase.Organizations
                         where U.ID == newData.ID
                         select U
                         ).FirstOrDefaultAsync();



        // Si el usuario no existe
        if (usuario == null)
        {
            return new ResponseBase(Responses.NotExistAccount);
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
    public async static Task<ResponseBase> UpdateState(int user, AccountStatus status, Conexión context)
    {

        // Encontrar el usuario
        var usuario = await (from U in context.DataBase.Accounts
                             where U.ID == user
                             select U).FirstOrDefaultAsync();

        // Si el usuario no existe
        if (usuario == null)
        {
            return new ResponseBase(Responses.NotExistAccount);
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
    public async static Task<ResponseBase> UpdateGender(int user, Genders genero, Conexión context)
    {

        // Encontrar el usuario
        var usuario = await (from U in context.DataBase.Accounts
                             where U.ID == user
                             select U).FirstOrDefaultAsync();

        // Si el usuario no existe
        if (usuario == null)
        {
            return new ResponseBase(Responses.NotExistAccount);
        }

        // Cambiar Contraseña
        usuario.Genero = genero;

        context.DataBase.SaveChanges();
        return new(Responses.Success);

    }



    /// <summary>
    /// Actualiza la visibilidad
    /// </summary>
    /// <param name="user">ID</param>
    /// <param name="visibility">Nueva visibilidad</param>
    /// <param name="context">Contexto de conexión con la BD</param>
    public async static Task<ResponseBase> UpdateVisibility(int user, AccountVisibility visibility, Conexión context)
    {

        // Encontrar el usuario
        var usuario = await (from U in context.DataBase.Accounts
                             where U.ID == user
                             select U).FirstOrDefaultAsync();

        // Si el usuario no existe
        if (usuario == null)
        {
            return new ResponseBase(Responses.NotExistAccount);
        }

        // Cambiar visibilidad
        usuario.Visibilidad = visibility;

        context.DataBase.SaveChanges();
        return new(Responses.Success);

    }



}