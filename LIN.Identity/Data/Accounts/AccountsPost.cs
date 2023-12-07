namespace LIN.Identity.Data;


internal static partial class Accounts
{


    #region Abstracciones


    /// <summary>
    /// Crea una nueva cuenta
    /// </summary>
    /// <param name="data">Modelo de la nueva cuenta</param>
    public static async Task<ReadOneResponse<AccountModel>> Create(AccountModel data)
    {

        // Obtiene la conexión
        var (context, connectionKey) = Conexión.GetOneConnection();

        var res = await Create(data, context);
        context.CloseActions(connectionKey);
        return res;

    }


    #endregion



    /// <summary>
    /// Crea una nueva cuenta
    /// </summary>
    /// <param name="data">Modelo de la cuenta</param>
    /// <param name="context">Contexto de base de datos</param>
    public static async Task<ReadOneResponse<AccountModel>> Create(AccountModel data, Conexión context)
    {

        // Identidad.
        data.Identity.Id = 0;
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


}