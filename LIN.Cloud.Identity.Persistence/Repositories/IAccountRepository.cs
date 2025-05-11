namespace LIN.Cloud.Identity.Persistence.Repositories;

public interface IAccountRepository
{
    Task<ReadOneResponse<AccountModel>> Create(AccountModel modelo, int organization);

    Task<ReadOneResponse<AccountModel>> Read(int id, QueryObjectFilter filters);

    Task<ReadOneResponse<AccountModel>> Read(string unique, QueryObjectFilter filters);

    Task<ReadOneResponse<AccountModel>> ReadByIdentity(int id, QueryObjectFilter filters);

    Task<ReadAllResponse<AccountModel>> Search(string pattern, QueryObjectFilter filters);

    Task<ReadAllResponse<AccountModel>> FindAll(List<int> ids, QueryObjectFilter filters);

    Task<ReadAllResponse<AccountModel>> FindAllByIdentities(List<int> ids, QueryObjectFilter filters);

    Task<ResponseBase> UpdatePassword(int accountId, string password);
}