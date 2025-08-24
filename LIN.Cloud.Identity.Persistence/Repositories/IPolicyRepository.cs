namespace LIN.Cloud.Identity.Persistence.Repositories;

public interface IPolicyRepository
{
    Task<CreateResponse> Create(PolicyModel policyModel);
    Task<CreateResponse> Add(TimeAccessPolicy policyModel);
    Task<CreateResponse> Add(IpAccessPolicy policyModel);
    Task<CreateResponse> Add(IdentityTypePolicy policyModel);
    Task<ReadOneResponse<PolicyModel>> Read(int id, bool includeDetails);
    Task<ReadAllResponse<PolicyModel>> ReadAll(int organization, bool includeDetails);
    Task<ReadAllResponse<PolicyModel>> ReadAll(int organization, string query);
    Task<ReadAllResponse<PolicyModel>> ReadAllByApp(int application, bool includeDetails);
    Task<ReadAllResponse<PolicyModel>> ReadAll(IEnumerable<int> identity, int organization, bool includeDetails);
    Task<ResponseBase> Delete(int id);
}