namespace LIN.Cloud.Identity.Persistence.Repositories;

public interface IPolicyMemberRepository
{
    Task<CreateResponse> Create(string policy, int identity);
    Task<ReadAllResponse<AccessPolicyModel>> ReadAll(int identity);
    Task<ResponseBase> Remove(string policy, int id);
}