namespace LIN.Identity.Data;


public class MailLinks
{


    #region Abstracciones


    /// <summary>
    /// Crea un nuevo LINK
    /// </summary>
    /// <param name="data">Modelo del link</param>
    public async static Task<CreateResponse> Create(MailMagicLink data)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();
        var res = await Create(data, context);
        context.CloseActions(connectionKey);
        return res;
    }



    /// <summary>
    /// Obtiene un link activo según su key
    /// </summary>
    /// <param name="value"></param>
    public async static Task<ReadOneResponse<MailMagicLink>> ReadAndDisable(string value)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await ReadAndDisable(value, context);
        context.CloseActions(connectionKey);
        return res;

    }


    #endregion



    /// <summary>
    /// Crea un nuevo enlace para email
    /// </summary>
    /// <param name="data">Modelo del link</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<CreateResponse> Create(MailMagicLink data, Conexión context)
    {
        // ID en 0
        data.ID = 0;

        // Ejecución
        try
        {
            var res = context.DataBase.MailMagicLinks.Add(data);
            await context.DataBase.SaveChangesAsync();

            return new(Responses.Success, data.ID);
        }
        catch
        {
            context.DataBase.Remove(data);
        }
        return new();
    }



    /// <summary>
    /// Obtiene un link activo según su key
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="context">Contexto de conexión</param>
    /// <param name="connectionKey">Llave para cerrar la conexión</param>
    public async static Task<ReadOneResponse<MailMagicLink>> ReadAndDisable(string key, Conexión context)
    {

        // Ejecución
        try
        {

            var now = DateTime.Now;
            var verification = await (from L in context.DataBase.MailMagicLinks
                                      where L.Key == key
                                      where L.Vencimiento > now
                                      where L.Status == MagicLinkStatus.Activated
                                      select L).FirstOrDefaultAsync();


            if (verification == null)
            {
                return new();
            }

            verification.Status = MagicLinkStatus.Deactivated;
            context.DataBase.SaveChanges();

            return new(Responses.Success, verification);
        }
        catch
        {
        }

        return new();
    }



}