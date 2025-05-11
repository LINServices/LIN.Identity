namespace LIN.Cloud.Identity.Persistence.Repositories;

public interface IAccountLogRepository
{
    Task<CreateResponse> Create(AccountLog log);
    Task<ReadAllResponse<AccountLog>> ReadAll(int accountId, DateTime? start, DateTime? end);
    Task<ReadOneResponse<int>> Count(int id);
}