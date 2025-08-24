namespace LIN.Cloud.Identity.Persistence.Repositories;

public interface IApplicationRepository
{
    Task<CreateResponse> Create(ApplicationModel modelo);
    Task<ReadOneResponse<ApplicationModel>> Read(string key);
    Task<ReadOneResponse<ApplicationModel>> Read(int key);
    Task<ReadOneResponse<bool>> ExistApp(string key);
}