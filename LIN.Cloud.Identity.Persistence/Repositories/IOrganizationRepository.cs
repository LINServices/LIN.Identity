namespace LIN.Cloud.Identity.Persistence.Repositories;

public interface IOrganizationRepository
{
    Task<CreateResponse> Create(OrganizationModel modelo);

    Task<ReadOneResponse<OrganizationModel>> Read(int id);

    Task<ReadOneResponse<IdentityModel>> GetDomain(int id);

    Task<ReadOneResponse<int>> ReadDirectory(int id);

    Task<ReadOneResponse<int>> ReadDirectoryIdentity(int id);
}