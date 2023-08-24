namespace LIN.Auth.Data.Accounts;


public static partial class Accounts
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



}