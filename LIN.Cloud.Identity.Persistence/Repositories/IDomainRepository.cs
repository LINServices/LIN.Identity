namespace LIN.Cloud.Identity.Persistence.Repositories;

public interface IDomainRepository
{
    Task<CreateResponse> Create(DomainModel modelo);
    Task<ReadOneResponse<DomainModel>> Read(string unique);
    Task<ResponseBase> Verify(string unique);
}