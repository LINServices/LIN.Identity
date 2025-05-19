namespace LIN.Cloud.Identity.Persistence.Repositories.EntityFramework;

internal class MailRepository(DataContext context) : IMailRepository
{

    /// <summary>
    /// Crear un nuevo mail.
    /// </summary>
    public async Task<ReadOneResponse<MailModel>> Create(MailModel model)
    {
        try
        {

            model.Id = 0;
            model.Account = new()
            {
                Id = model.AccountId,
            };

            // Attach.
            context.Attach(model.Account);

            // Guardar la cuenta.
            await context.Mails.AddAsync(model);
            context.SaveChanges();

            return new(Responses.Success, model);
        }
        catch (Exception)
        {
            return new(Responses.ResourceExist);
        }
    }


    /// <summary>
    /// Obtener el correo principal de una identidad.
    /// </summary>
    public async Task<ReadOneResponse<MailModel>> ReadPrincipal(string unique)
    {
        try
        {

            var mailModel = await (from mail in context.Mails
                                   join account in context.Accounts
                                   on mail.Account.IdentityId equals account.IdentityId
                                   where account.Identity.Unique == unique
                                   where mail.IsPrincipal
                                   && mail.IsVerified
                                   select mail).FirstOrDefaultAsync();

            if (mailModel is null)
                return new(Responses.NotRows);

            return new(Responses.Success, mailModel);
        }
        catch (Exception)
        {
            return new();
        }

    }


    /// <summary>
    /// Validar un código OTP para un correo.
    /// </summary>
    /// <param name="email">Correo electrónico.</param>
    /// <param name="code">Código OTP.</param>
    public async Task<ResponseBase> ValidateOtpForMail(string email, string code)
    {
        try
        {
            // Obtener model.
            var otpModel = (from mail in context.Mails
                            where mail.Mail == email
                            join otp in context.MailOtp
                            on mail.Id equals otp.MailId
                            where otp.OtpDatabaseModel.Code == code
                            && otp.OtpDatabaseModel.IsUsed == false
                            && otp.OtpDatabaseModel.ExpireTime > DateTime.UtcNow
                            select otp);

            // Actualizar.
            await otpModel.Select(t => t.MailModel).ExecuteUpdateAsync(t => t.SetProperty(t => t.IsVerified, true));
            int countUpdate = await otpModel.Select(t => t.OtpDatabaseModel).ExecuteUpdateAsync(t => t.SetProperty(t => t.IsUsed, true));

            // Si no se actualizaron.
            if (countUpdate <= 0)
                return new(Responses.NotRows);

            return new(Responses.Success);
        }
        catch (Exception)
        {
            return new(Responses.Undefined);
        }
    }

}