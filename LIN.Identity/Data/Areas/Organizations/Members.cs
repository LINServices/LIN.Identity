namespace LIN.Identity.Data.Areas.Organizations;


public class Members
{


    #region Abstracciones




    /// <summary>
    /// Obtiene la lista de integrantes de una organización
    /// </summary>
    /// <param name="id">ID de la organización</param>
    public static async Task<ReadAllResponse<AccountModel>> ReadAll(int id)
    {
        var (context, contextKey) = Conexión.GetOneConnection();

        var res = await ReadAll(id, context);
        context.CloseActions(contextKey);
        return res;
    }




    public static async Task<ReadOneResponse<AccountModel>> Create(AccountModel data, int dir, Types.Identity.Enumerations.Roles rol)
    {

        var (context, contextKey) = Conexión.GetOneConnection();
        var res = await Create(data, dir, rol, context);
        context.CloseActions(contextKey);
        return res;
    }


    #endregion



    /// <summary>
    /// Crea una cuenta en una organización
    /// </summary>
    /// <param name="data">Modelo</param>
    /// <param name="rol">Rol dentro de la organización</param>
    /// <param name="context">Contexto de conexión</param>
    public static async Task<ReadOneResponse<AccountModel>> Create(AccountModel data, int directory, Types.Identity.Enumerations.Roles rol, Conexión context)
    {

        data.ID = 0;

        // Ejecución
        using (var transaction = context.DataBase.Database.BeginTransaction())
        {
            try
            {

                // Guardar la cuenta.
                context.DataBase.Accounts.Add(data);
                context.DataBase.SaveChanges();

                // Obtiene la organización.
                int directoryId = await (from org in context.DataBase.Organizations
                                         where org.DirectoryId == directory
                                         select org.DirectoryId).FirstOrDefaultAsync();

                // No existe la organización.
                if (directoryId <= 0)
                {
                    transaction.Rollback();
                    return new(Responses.NotRows);
                }

                // Modelo del integrante.
                var member = new DirectoryMember()
                {
                    Directory = new()
                    {
                        ID = directoryId
                    },
                    Identity = data.Identity
                };

                // El directorio ya existe.
                context.DataBase.Attach(member.Directory);

                // Guardar cambios.
                context.DataBase.DirectoryMembers.Add(member);
                context.DataBase.SaveChanges();

                // Enviar la transacción.
                transaction.Commit();

                return new(Responses.Success, data);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                if ((ex.InnerException?.Message.Contains("Violation of UNIQUE KEY constraint") ?? false) || (ex.InnerException?.Message.Contains("duplicate key") ?? false))
                    return new(Responses.ExistAccount);

            }
        }

        return new();
    }



    /// <summary>
    /// Obtiene la lista de integrantes de una organización.
    /// </summary>
    /// <param name="id">ID de la organización</param>
    /// <param name="context">Contexto de conexión</param>
    public static async Task<ReadAllResponse<AccountModel>> ReadAll(int id, Conexión context)
    {

        // Ejecución
        try
        {

            // Organización
            var members = from org in context.DataBase.Organizations
                          where org.ID == id
                          join m in context.DataBase.DirectoryMembers
                          on org.DirectoryId equals m.DirectoryId
                          select org.Directory;


            var accounts = await (from account in context.DataBase.Accounts
                                  join m in members
                                  on account.IdentityId equals m.IdentityId
                                  select new AccountModel
                                  {
                                      Creación = account.Creación,
                                      ID = account.ID,
                                      Nombre = account.Nombre,
                                      Identity = new()
                                      {
                                          Id = account.Identity.Id,
                                          Type = IdentityTypes.Account,
                                          Unique = account.Identity.Unique
                                      }
                                  }).ToListAsync();

            return new(Responses.Success, accounts);
        }
        catch
        {
        }

        return new();
    }


}