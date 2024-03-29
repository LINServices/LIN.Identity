namespace LIN.Cloud.Identity.Data;


public partial class Accounts
{


    /// <summary>
    /// Crear nueva cuenta.
    /// </summary>
    /// <param name="modelo">Modelo de la cuenta.</param>
    /// <param name="organization">Id de la organización.</param>
    public static async Task<ReadOneResponse<AccountModel>> Create(AccountModel modelo, int organization)
    {

        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await Create(modelo, context, organization);

        // Retornar.
        context.Close(contextKey);
        return response;

    }



    /// <summary>
    /// Obtener una cuenta según el Id.
    /// </summary>
    /// <param name="id">Id de la cuenta.</param>
    /// <param name="filters">Filtros de búsqueda.</param>
    public static async Task<ReadOneResponse<AccountModel>> Read(int id, Services.Models.QueryAccountFilter filters)
    {

        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await Read(id, filters, context);

        // Retornar.
        context.Close(contextKey);
        return response;

    }



    /// <summary>
    /// Obtener una cuenta según el identificador único.
    /// </summary>
    /// <param name="unique">Único.</param>
    /// <param name="filters">Filtros de búsqueda.</param>
    public static async Task<ReadOneResponse<AccountModel>> Read(string unique, Services.Models.QueryAccountFilter filters)
    {

        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await Read(unique, filters, context);

        // Retornar.
        context.Close(contextKey);
        return response;

    }



    /// <summary>
    /// Obtener una cuenta según el identificador único.
    /// </summary>
    /// <param name="id">Id de la identidad.</param>
    /// <param name="filters">Filtros de búsqueda.</param>
    public static async Task<ReadOneResponse<AccountModel>> ReadByIdentity(int id, Services.Models.QueryAccountFilter filters)
    {
        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await ReadByIdentity(id, filters, context);

        // Retornar.
        context.Close(contextKey);
        return response;

    }



    /// <summary>
    /// Buscar por patron.
    /// </summary>
    /// <param name="pattern">patron de búsqueda</param>
    /// <param name="filters">Filtros</param>
    public static async Task<ReadAllResponse<AccountModel>> Search(string pattern, QueryAccountFilter filters)
    {
        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await Search(pattern, filters, context);

        // Retornar.
        context.Close(contextKey);
        return response;
    }



    /// <summary>
    /// Obtiene los usuarios con IDs coincidentes
    /// </summary>
    /// <param name="ids">Lista de IDs</param>
    /// <param name="filters">Filtros.</param>
    public static async Task<ReadAllResponse<AccountModel>> FindAll(List<int> ids, QueryAccountFilter filters)
    {
        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await FindAll(ids, filters, context);

        // Retornar.
        context.Close(contextKey);
        return response;
    }


}