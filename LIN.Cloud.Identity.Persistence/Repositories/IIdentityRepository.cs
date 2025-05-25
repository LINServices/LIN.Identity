namespace LIN.Cloud.Identity.Persistence.Repositories;

public interface IIdentityRepository
{
    Task<ReadOneResponse<IdentityModel>> Create(IdentityModel modelo);
    Task<ReadOneResponse<IdentityModel>> Read(int id, QueryIdentityFilter filters);
    Task<ReadOneResponse<bool>> Exist(string unique);
    Task<ReadOneResponse<IdentityModel>> Read(string unique, QueryIdentityFilter filters);
}