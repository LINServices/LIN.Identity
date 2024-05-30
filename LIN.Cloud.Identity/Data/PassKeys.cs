using LIN.Cloud.Identity.Persistence.Models;

namespace LIN.Cloud.Identity.Data;


public static class PassKeys
{



    #region Abstracciones



    /// <summary>
    /// Crear nueva identidad.
    /// </summary>
    /// <param name="modelo">Modelo.</param>
    public static async Task<ResponseBase> Create(PassKeyDBModel modelo)
    {

        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await Create(modelo, context);

        // Retornar.
        context.Close(contextKey);
        return response;

    }



    /// <summary>
    /// Obtener una identidad según el Id.
    /// </summary>
    /// <param name="id">Id.</param>
    /// <param name="filters">Filtros de búsqueda.</param>
    public static async Task<ReadOneResponse<int>> Count(int id)
    {

        // Obtener conexión.
        var (context, contextKey) = DataService.GetConnection();

        // Función.
        var response = await Count(id, context);

        // Retornar.
        context.Close(contextKey);
        return response;

    }





    #endregion



    
    public static async Task<ResponseBase> Create(PassKeyDBModel modelo, DataContext context)
    {
        // Pre.
        modelo.Id = 0;

        try
        {

            // Guardar la identidad.
            await context.PassKeys.AddAsync(modelo);
            context.SaveChanges();

            return new()
            {
                Response = Responses.Success,
            };

        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.ExistAccount
            };
        }

    }



    
    public static async Task<ReadOneResponse<int>> Count(int id, DataContext context)
    {

        try
        {


            var time = DateTime.Now;


            var c = await (from a in context.PassKeys
                           where a.AccountId == id
                           where a.Time.Year == time.Year
                           && a.Time.Month == time.Month
                           && a.Time.Day == time.Day
                           select a).CountAsync();



            // Success.
            return new()
            {
                Response = Responses.Success,
                Model = c
            };

        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.NotRows
            };
        }

    }




}