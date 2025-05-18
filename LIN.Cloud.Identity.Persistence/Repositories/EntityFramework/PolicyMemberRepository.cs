namespace LIN.Cloud.Identity.Persistence.Repositories.EntityFramework;

internal class PolicyMemberRepository(DataContext context) : IPolicyMemberRepository
{

    /// <summary>
    /// Crear nueva política de acceso general.
    /// </summary>
    public async Task<CreateResponse> Create(IdentityPolicyModel model)
    {
        try
        {
            model.Identity = context.AttachOrUpdate(model.Identity)!;
            model.Policy = context.AttachOrUpdate(model.Policy)!;

            // Guardar la cuenta.
            await context.IdentityPolicies.AddAsync(model);
            context.SaveChanges();

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


    public async Task<ReadAllResponse<PolicyModel>> ReadAll(int id)
    {
        try
        {

            var identities = await(from pl in context.IdentityPolicies
                             where pl.IdentityId == id
                             select pl.Policy).ToListAsync();

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

}