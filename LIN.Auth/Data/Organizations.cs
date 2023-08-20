namespace LIN.Auth.Data;


public class Organizations
{


    #region Abstracciones


    /// <summary>
    /// Crea una organización
    /// </summary>
    /// <param name="data">Modelo</param>
    public async static Task<CreateResponse> Create(OrganizationModel data)
    {
        var (context, contextKey) = Conexión.GetOneConnection();
        var response = await Create(data, context);
        context.CloseActions(contextKey);
        return response;
    }



    /// <summary>
    /// Obtiene una organización
    /// </summary>
    /// <param name="id">ID de la organización</param>
    public async static Task<ReadOneResponse<OrganizationModel>> Read(int id)
    {
        var (context, contextKey) = Conexión.GetOneConnection();

        var res = await Read(id, context);
        context.CloseActions(contextKey);
        return res;
    }


    #endregion



    /// <summary>
    /// Crear una organización
    /// </summary>
    /// <param name="data">Modelo</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<CreateResponse> Create(OrganizationModel data, Conexión context)
    {

        data.ID = 0;

        // Ejecución
        try
        {

            var res = await context.DataBase.Organizations.AddAsync(data);
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
    /// Obtiene una organización
    /// </summary>
    /// <param name="id">ID de la organización</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadOneResponse<OrganizationModel>> Read(int id, Conexión context)
    {

        // Ejecución
        try
        {

            // Query
            var email = await (from E in context.DataBase.Organizations
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


}