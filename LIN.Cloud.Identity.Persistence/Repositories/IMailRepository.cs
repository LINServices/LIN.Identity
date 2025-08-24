namespace LIN.Cloud.Identity.Persistence.Repositories;

public interface IMailRepository
{
    Task<ReadOneResponse<MailModel>> Create(MailModel model);
    Task<ReadOneResponse<MailModel>> ReadPrincipal(string unique);
    Task<ResponseBase> ValidateOtpForMail(string email, string code);
}