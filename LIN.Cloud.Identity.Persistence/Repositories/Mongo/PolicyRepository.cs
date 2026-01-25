using MongoDB.Bson;

namespace LIN.Cloud.Identity.Persistence.Repositories.Mongo;

public class PolicyRepository(MongoDataContext context)
{
    /// <summary>
    /// Crea una nueva política.
    /// </summary>
    public async Task<CreateResponse> Create(AccessPolicyModel model)
    {
        try
        {
            model.Id = Guid.CreateVersion7().ToString();
            await context.AccessPolicies.AddAsync(model);
            await context.SaveChangesAsync();
            return new(Responses.Success)
            {
                LastUnique = model.Id
            };
        }
        catch (Exception)
        {
            return new(Responses.Undefined);
        }
    }


    /// <summary>
    /// Elimina una política.
    /// </summary>
    public async Task<ResponseBase> Delete(string id)
    {
        try
        {;

            await context.AccessPolicies.Where(t => t.Id == id).ExecuteDeleteAsync();
            return new(Responses.Success);
        }
        catch (Exception)
        {
            return new(Responses.Undefined);
        }
    }


    /// <summary>
    /// Obtiene las políticas de una identidad.
    /// </summary>
    public async Task<ReadAllResponse<AccessPolicyModel>> ReadAll(int identityId)
    {
        try
        {
            var policies = await context.AccessPolicies
                .Where(t => t.Identities.Contains(identityId))
                .ToListAsync();

            return new(Responses.Success, policies);
        }
        catch (Exception)
        {
            return new(Responses.Undefined);
        }
    }

    public async Task<ReadAllResponse<AccessPolicyModel>> ReadAllByOrg(int org, string? query)
    {
        try
        {
            var policies = await context.AccessPolicies
                .Where(t => t.OrganizationId == org &&
                (query == null || t.Name.ToLower().Contains(query.ToLower())))
                .ToListAsync();

            return new(Responses.Success, policies);
        }
        catch (Exception)
        {
            return new(Responses.Undefined);
        }
    }


    /// <summary>
    /// Obtiene las políticas para un conjunto de identidades (útil para jerarquías).
    /// </summary>
    public async Task<List<AccessPolicyModel>> ReadAll(IEnumerable<int> identities)
    {
        try
        {
            return await context.AccessPolicies
                 .Where(t => t.Identities.Any(i => identities.Contains(i)))
                 .ToListAsync();
        }
        catch (Exception)
        {
            return [];
        }
    }
}
