using LIN.Cloud.Identity.Persistence.Models;

namespace LIN.Cloud.Identity.Data;

public class OtpService(DataContext context)
{

    public async Task<CreateResponse> Create(OtpDatabaseModel model)
    {

        try
        {
            // Guardar OTP.
            await context.OTPs.AddAsync(model);
            context.SaveChanges();

            return new(Responses.Success);

        }
        catch (Exception)
        {
        }
        return new(Responses.Undefined);

    }


    public async Task<CreateResponse> ReadAndUpdate(int accountId, string code)
    {

        try
        {


            var update = await (from A in context.OTPs
                                where A.AccountId == accountId
                                && A.Code == code
                                && A.ExpireTime > DateTime.Now
                                && A.IsUsed == false
                                select A).ExecuteUpdateAsync(t => t.SetProperty(t => t.IsUsed, true));


            if (update <= 0)
                return new(Responses.NotRows);


            return new(Responses.Success);

        }
        catch (Exception)
        {
        }
        return new(Responses.Undefined);

    }



}