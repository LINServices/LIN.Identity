namespace LIN.Cloud.Identity.Persistence.Repositories;

public interface IPolicyMemberRepository
{
    Task<CreateResponse> Create(IdentityPolicyModel policyModel);
}