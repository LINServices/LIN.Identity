namespace LIN.Cloud.Identity.Data;

public class Mails(DataContext context)
{

    public async Task<ReadOneResponse<MailModel>> Create(MailModel modelo)
    {
        try
        {

            modelo.Id = 0;
            modelo.Account = new()
            {
                Id = modelo.AccountId,
            };

            // Attach.
            context.Attach(modelo.Account);

            // Guardar la cuenta.
            await context.Mails.AddAsync(modelo);
            context.SaveChanges();

            return new()
            {
                Response = Responses.Success,
                Model = modelo
            };

        }
        catch (Exception ex)
        {


            return new()
            {
                Response = Responses.ResourceExist
            };
        }

        return new()
        {
            Response = Responses.Undefined
        };

    }


    public async Task<ReadOneResponse<MailModel>> ReadPrincipal(string unique)
    {
        try
        {

            var x = await (from mail in context.Mails
                           join account in context.Accounts
                           on mail.Account.IdentityId equals account.IdentityId
                           where account.Identity.Unique == unique
                           where mail.IsPrincipal
                           && mail.IsVerified
                           select mail).FirstOrDefaultAsync();

            if (x is null)
                return new()
                {
                    Response = Responses.NotRows
                };


            return new()
            {
                Response = Responses.Success,
                Model = x
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


    public async Task<ResponseBase> ValidateOtpFormail(string email, string code)
    {
        try
        {

            var x = (from mail in context.Mails
                     join otp in context.MailOtp
                     on mail.Id equals otp.MailId
                     where otp.OtpDatabaseModel.Code == code
                     && otp.OtpDatabaseModel.IsUsed == false
                     && otp.OtpDatabaseModel.ExpireTime > DateTime.Now
                     select otp);


            var xx = await x.Select(t => t.MailModel).ExecuteUpdateAsync(t => t.SetProperty(t => t.IsVerified, true));
            var xx2 = await x.Select(t=>t.OtpDatabaseModel).ExecuteUpdateAsync(t => t.SetProperty(t => t.IsUsed, true));


            if (xx2 <= 0)
                return new()
                {
                    Response = Responses.NotRows
                };


            var acf =await (from mail in context.Mails
                     select mail.AccountId).FirstOrDefaultAsync();


            var exist = await (from m in context.Mails
                        where m.AccountId == acf
                        && m.IsPrincipal
                        select m).AnyAsync();

            if (!exist)
                await x.Select(t => t.MailModel).ExecuteUpdateAsync(t => t.SetProperty(t => t.IsPrincipal, true));


            return new()
            {
                Response = Responses.Success
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



}