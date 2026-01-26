using MongoDB.Driver;

namespace LIN.Cloud.Identity.Persistence.Repositories.EntityFramework;

internal class PolicyMemberRepository(MongoDataContext context) : IPolicyMemberRepository
{

    /// <summary>
    /// Crear nueva política de acceso general.
    /// </summary>
    public async Task<CreateResponse> Create(string policy, int identity)
    {
        try
        {
            var policyModel = await context.AccessPolicies.FirstOrDefaultAsync(t => t.Id == policy);


            if (policyModel != null)
            {
                policyModel.Identities.Add(identity);
                context.SaveChanges();
            }

            return new()
            {
                Response = Responses.Success
            };
        }
        catch (Exception)
        {
        }
        return new();
    }


    public async Task<ReadAllResponse<AccessPolicyModel>> ReadAll(int id)
    {
        try
        {

            var identities = await (from pl in context.AccessPolicies
                                    where pl.Identities.Contains(id)
                                    select pl).ToListAsync();

            return new()
            {
                Response = Responses.Success,
                Models = identities
            };
        }
        catch (Exception)
        {
        }
        return new();
    }

    public async Task<ResponseBase> Remove(string policy, int id)
    {
        try
        {
            var entity = await context.AccessPolicies.FirstOrDefaultAsync(x => x.Id == policy);

            if (entity != null)
            {
                entity.Identities.Remove(id);
                await context.SaveChangesAsync();
            }

            return new()
            {
                Response = Responses.Success
            };
        }
        catch (Exception)
        {
        }
        return new();
    }
}