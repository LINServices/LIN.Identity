namespace LIN.Cloud.Identity.Data;

public class AccountLogs (DataContext context)
{
  
    /// <summary>
    /// Crear nuevo log.
    /// </summary>
    /// <param name="log">Modelo.</param>
    public async Task<CreateResponse> Create(AccountLog log)
    {

        // Pre.
        log.Id = 0;

        try
        {
            log.Account = new()
            {
                Id = log.AccountId
            };
            context.Attach(log.Account);
            log.Application = null;
            log.ApplicationId = null;

            // Guardar la cuenta.
            await context.AccountLogs.AddAsync(log);
            context.SaveChanges();

            return new()
            {
                Response = Responses.Success,
                LastID = log.Id
            };

        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.Undefined
            };
        }

    }


 
    public async Task<ReadAllResponse<AccountLog>> ReadAll(int accountId)
    {

        try
        {

            var x = await context.AccountLogs
                                 .Where(t => t.AccountId == accountId)
                                 .OrderByDescending(t => t.Time)
                                 .Take(20)
                                 .ToListAsync();

            return new()
            {
                Models = x,
                Response = Responses.Success
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

}