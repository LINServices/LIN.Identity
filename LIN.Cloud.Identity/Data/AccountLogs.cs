namespace LIN.Cloud.Identity.Data;

public class AccountLogs(DataContext context)
{

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

            if (log.Application is null)
            {
                log.Application = null;
                log.ApplicationId = null;
            }
            else
            {
                log.Application = context.AttachOrUpdate(log.Application);
            }

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