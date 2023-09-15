namespace LIN.Auth.Data;


public class Mails
{


    #region Abstracciones


    /// <summary>
    /// Crea un nuevo email
    /// </summary>
    /// <param name="data">Modelo</param>
    public async static Task<CreateResponse> Create(EmailModel data)
    {
        var (context, contextKey) = Conexión.GetOneConnection();
        var response = await Create(data, context);
        context.CloseActions(contextKey);
        return response;
    }



    /// <summary>
    /// Obtiene la lista de emails asociados a una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    public async static Task<ReadAllResponse<EmailModel>> ReadAll(int id)
    {
        var (context, contextKey) = Conexión.GetOneConnection();

        var res = await ReadAll(id, context);
        context.CloseActions(contextKey);
        return res;
    }



    /// <summary>
    /// Obtiene un email
    /// </summary>
    /// <param name="id">ID del email</param>
    public async static Task<ReadOneResponse<EmailModel>> Read(int id)
    {
        var (context, contextKey) = Conexión.GetOneConnection();

        var res = await Read(id, context);
        context.CloseActions(contextKey);
        return res;
    }



    /// <summary>
    /// Obtiene la lista de emails verificados asociados a una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    public async static Task<ReadAllResponse<EmailModel>> ReadVerifiedEmails(int id)
    {
        var (context, contextKey) = Conexión.GetOneConnection();
        var res = await ReadVerifiedEmails(id, context);
        context.CloseActions(contextKey);
        return res;
    }



    /// <summary>
    /// Establece el email default de una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="emailID">ID de el email</param>
    public async static Task<ResponseBase> SetDefaultEmail(int id, int emailID)
    {
        var (context, contextKey) = Conexión.GetOneConnection();

        var res = await SetDefaultEmail(id, emailID, context);
        context.CloseActions(contextKey);
        return res;
    }



    /// <summary>
    /// Actualiza el estado de un email
    /// </summary>
    /// <param name="id">ID del email</param>
    /// <param name="state">Nuevo estado</param>
    public async static Task<ResponseBase> UpdateState(int id, EmailStatus state)
    {
        var (context, contextKey) = Conexión.GetOneConnection();
        var res = await UpdateState(id, state, context);
        context.CloseActions(contextKey);
        return res;
    }



    #endregion



    /// <summary>
    /// Crea un nuevo email
    /// </summary>
    /// <param name="data">Modelo</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<CreateResponse> Create(EmailModel data, Conexión context)
    {

        data.ID = 0;

        // Ejecución
        try
        {

            var res = await context.DataBase.Emails.AddAsync(data);
            context.DataBase.SaveChanges();

            return new(Responses.Success, data.ID);
        }
        catch (Exception ex)
        {
            
            if ((ex.InnerException?.Message.Contains("Violation of UNIQUE KEY constraint") ?? false) || (ex.InnerException?.Message.Contains("duplicate key") ?? false))
                return new(Responses.Undefined);

        }

        return new();
    }



    /// <summary>
    /// Obtiene la lista de emails asociados a una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadAllResponse<EmailModel>> ReadAll(int id, Conexión context)
    {

        // Ejecución
        try
        {

            // Query
            var emails = await (from E in context.DataBase.Emails
                                where E.UserID == id
                                where E.Status != EmailStatus.Delete
                                select E).ToListAsync();

            return new(Responses.Success, emails);
        }
        catch 
        {
        }

        return new();
    }



    /// <summary>
    /// Obtiene un email
    /// </summary>
    /// <param name="id">ID de el email</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadOneResponse<EmailModel>> Read(int id, Conexión context)
    {

        // Ejecución
        try
        {

            // Query
            var email = await (from E in context.DataBase.Emails
                               where E.ID == id
                               select E).FirstOrDefaultAsync();

            // Email no existe
            if (email == null)
            {
                return new(Responses.NotRows);
            }

            return new(Responses.Success, email);
        }
        catch 
        {
        }

        return new();
    }



    /// <summary>
    /// Obtiene la lista de emails verificados asociados a una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadAllResponse<EmailModel>> ReadVerifiedEmails(int id, Conexión context)
    {

        // Ejecución
        try
        {

            // Query
            var emails = await (from E in context.DataBase.Emails
                                where E.UserID == id && E.Status == EmailStatus.Verified
                                select E).ToListAsync();

            return new(Responses.Success, emails);
        }
        catch
        {
           
        }

        return new();
    }



    /// <summary>
    /// Establece el email default de una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="emailID">ID de el email</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ResponseBase> SetDefaultEmail(int id, int emailID, Conexión context)
    {

        // Ejecución (Transacción)
        using (var transaction = context.DataBase.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
        {
            try
            {
                // Query
                var emails = await (from E in context.DataBase.Emails
                                    where E.UserID == id && E.Status == EmailStatus.Verified
                                    select E).ToListAsync();


                var actualDefault = emails.Where(T => T.IsDefault).FirstOrDefault();
                if (actualDefault != null)
                {
                    actualDefault.IsDefault = false;
                }

                var seted = emails.Where(T => T.ID == emailID && T.Status == EmailStatus.Verified).FirstOrDefault();

                if (seted == null)
                {
                    transaction.Rollback();
                    return new(Responses.NotRows);
                }

                seted.IsDefault = true;
                context.DataBase.SaveChanges();
                transaction.Commit();

                return new(Responses.Success);
            }
            catch
            {
                transaction.Rollback();
            }
        }

        return new();
    }



    /// <summary>
    /// Actualiza el estado de un email
    /// </summary>
    /// <param name="id">ID de el email</param>
    /// <param name="state">Nuevo estado</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ResponseBase> UpdateState(int id, EmailStatus state, Conexión context)
    {

        // Ejecución
        try
        {

            // Query
            var email = await (from E in context.DataBase.Emails
                               where E.ID == id
                               select E).FirstOrDefaultAsync();

            // Email no existe
            if (email == null)
            {
                return new(Responses.NotRows);
            }

            email.Status = state;
            context.DataBase.SaveChanges();

            return new(Responses.Success);
        }
        catch 
        {
        }

        return new();
    }



}