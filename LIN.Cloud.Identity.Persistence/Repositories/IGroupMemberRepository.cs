namespace LIN.Cloud.Identity.Persistence.Repositories;

public interface IGroupMemberRepository
{
    Task<CreateResponse> Create(GroupMember modelo);
    Task<CreateResponse> Create(IEnumerable<GroupMember> modelos);
    Task<ReadAllResponse<GroupMember>> ReadAll(int id);
    Task<ReadAllResponse<IdentityModel>> Search(string pattern, int group);
    Task<ResponseBase> Delete(int identity, int group);
    Task<ReadAllResponse<GroupModel>> OnMembers(int organization, int identity);
}