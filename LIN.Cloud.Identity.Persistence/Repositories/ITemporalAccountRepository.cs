namespace LIN.Cloud.Identity.Persistence.Repositories;

public interface ITemporalAccountRepository
{
    Task<CreateResponse> Create(TemporalAccountModel modelo);
    Task<ResponseBase> Delete(int id);
    Task<ReadOneResponse<TemporalAccountModel>> ReadWithCode(string verificationCode);
}