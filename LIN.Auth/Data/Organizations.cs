﻿namespace LIN.Auth.Data;


public class Organizations
{


    #region Abstracciones


    /// <summary>
    /// Crea una organización
    /// </summary>
    /// <param name="data">Modelo</param>
    public async static Task<ReadOneResponse<OrganizationModel>> Create(OrganizationModel data, int account)
    {
        var (context, contextKey) = Conexión.GetOneConnection();
        var response = await Create(data, account, context);
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
    public async static Task<ReadOneResponse<OrganizationModel>> Create(OrganizationModel data, int user, Conexión context)
    {

        data.ID = 0;

        // Ejecución
        using (var transaction = context.DataBase.Database.BeginTransaction())
        {
            try
            {

                var res = await context.DataBase.Organizations.AddAsync(data);


                var account = (from A in context.DataBase.Accounts
                               where A.ID == user
                               select A).FirstOrDefault();

                if (account == null)
                {
                    return new ReadOneResponse<OrganizationModel>
                    {
                        Response = Responses.NotExistAccount
                    };
                }


                account.OrganizationAccess = new()
                {
                    Member = account,
                    Organization = data
                };

                context.DataBase.SaveChanges();

                transaction.Commit();

                return new(Responses.Success, data);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                if ((ex.InnerException?.Message.Contains("Violation of UNIQUE KEY constraint") ?? false) || (ex.InnerException?.Message.Contains("duplicate key") ?? false))
                    return new(Responses.Undefined);

            }
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