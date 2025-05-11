namespace LIN.Cloud.Identity.Persistence.Repositories;

public interface IOtpRepository
{
    Task<CreateResponse> Create(OtpDatabaseModel model);
    Task<CreateResponse> Create(MailOtpDatabaseModel model);
    Task<ResponseBase> ReadAndUpdate(int accountId, string code);
}