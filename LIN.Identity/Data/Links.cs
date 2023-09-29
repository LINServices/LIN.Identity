namespace LIN.Identity.Data;


public class Links
{


    #region Abstracciones


    /// <summary>
    /// Crea un nuevo LINK
    /// </summary>
    /// <param name="data">Modelo del link</param>
    public static async Task<CreateResponse> Create(UniqueLink data)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await Create(data, context);
        context.CloseActions(connectionKey);
        return res;
    }



    /// <summary>
    /// Obtiene la lista de links asociados a una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    public static async Task<ReadAllResponse<UniqueLink>> ReadAll(int id)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await ReadAll(id, context);
        context.CloseActions(connectionKey);
        return res;

    }



    /// <summary>
    /// Obtiene un link y cambia su estado
    /// </summary>
    /// <param name="value"></param>
    public static async Task<ReadOneResponse<UniqueLink>> ReadOneAnChange(string value)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await ReadOneAnChange(value, context);
        context.CloseActions(connectionKey);
        return res;

    }


    #endregion


    /// <summary>
    /// Crea un nuevo enlace
    /// </summary>
    /// <param name="data">Modelo del enlace</param>
    /// <param name="context">Contexto de conexión</param>
    public static async Task<CreateResponse> Create(UniqueLink data, Conexión context)
    {
        // ID en 0
        data.ID = 0;

        // Ejecución
        try
        {
            var res = context.DataBase.UniqueLinks.Add(data);
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
    /// Obtiene la lista de links activos asociados a una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="context">Contexto de conexión</param>
    public static async Task<ReadAllResponse<UniqueLink>> ReadAll(int id, Conexión context)
    {

        // Ejecución
        try
        {

            var now = DateTime.Now;

            var activos = await (from L in context.DataBase.UniqueLinks
                                 where L.AccountID == id
                                 where L.Vencimiento > now
                                 where L.Status == MagicLinkStatus.Activated
                                 select L).ToListAsync();

            var lista = activos;

            return new(Responses.Success, lista);
        }
        catch
        {
        }
        return new();

    }



    /// <summary>
    /// Obtiene un Magic Link y cambia su estado
    /// </summary>
    /// <param name="value"></param>
    /// <param name="context">Contexto de conexión</param>
    public static async Task<ReadOneResponse<UniqueLink>> ReadOneAnChange(string value, Conexión context)
    {

        // Ejecución
        try
        {

            // Fecha actual
            var now = DateTime.Now;

            // Consulta
            var elemento = await (from L in context.DataBase.UniqueLinks
                                  where L.Vencimiento > now
                                  where L.Status == MagicLinkStatus.Activated
                                  where L.Key == value
                                  select L).FirstOrDefaultAsync();
            // SI es null
            if (elemento == null)
                return new(Responses.NotRows);

            // Cambia el estado
            elemento.Status = MagicLinkStatus.None;
            context.DataBase.SaveChanges();

            return new(Responses.Success, elemento);
        }
        catch
        {
        }
        return new();
    }



}