namespace LIN.Cloud.Identity.Persistence.Repositories;

public interface IIdentityRolesRepository
{
    Task<ResponseBase> Create(IdentityRolesModel modelo);
    Task<ReadAllResponse<IdentityRolesModel>> ReadAll(int identity, int organization);
    Task<ResponseBase> Remove(int identity, Roles rol, int organization);
}