namespace LIN.Cloud.Identity.Persistence.Repositories.EntityFramework;

internal class PolicyRepository(DataContext context) : IPolicyRepository
{

    /// <summary>
    /// Crear nueva política de acceso general.
    /// </summary>
    public async Task<CreateResponse> Create(PolicyModel model)
    {
        try
        {
            model.CreatedBy = context.AttachOrUpdate(model.CreatedBy)!;
            model.Owner = context.AttachOrUpdate(model.Owner);

            // Guardar la cuenta.
            await context.Policies.AddAsync(model);
            context.SaveChanges();

            return new()
            {
                Response = Responses.Success,
                LastId = model.Id
            };
        }
        catch (Exception)
        {
        }
        return new();
    }


    /// <summary>
    /// Agregar una política de acceso por tiempo.
    /// </summary>
    public async Task<CreateResponse> Add(TimeAccessPolicy policyModel)
    {
        try
        {
            policyModel.Policy = context.AttachOrUpdate(policyModel.Policy)!;

            context.TimeAccessPolicies.Add(policyModel);
            await context.SaveChangesAsync();

            return new(Responses.Success, policyModel.Id);
        }
        catch (Exception)
        {
        }
        return new();
    }


    /// <summary>
    /// Agregar una política de acceso por IP.
    /// </summary>
    public async Task<CreateResponse> Add(IpAccessPolicy policyModel)
    {
        try
        {
            policyModel.Policy = context.AttachOrUpdate(policyModel.Policy)!;

            context.IpAccessPolicies.Add(policyModel);
            await context.SaveChangesAsync();

            return new(Responses.Success, policyModel.Id);
        }
        catch (Exception)
        {
        }
        return new();
    }


    /// <summary>
    /// Agregar una política de acceso por tipo de identidad.
    /// </summary>
    public async Task<CreateResponse> Add(IdentityTypePolicy policyModel)
    {
        try
        {
            policyModel.Policy = context.AttachOrUpdate(policyModel.Policy)!;

            context.IdentityTypesPolicies.Add(policyModel);
            await context.SaveChangesAsync();

            return new(Responses.Success, policyModel.Id);
        }
        catch (Exception)
        {
        }
        return new();
    }


    /// <summary>
    /// Eliminar una política de acceso.
    /// </summary>
    /// <param name="id">Id de la política.</param>
    public async Task<ResponseBase> Delete(int id)
    {
        try
        {
            // Eliminar las políticas de acceso.
            await context.IpAccessPolicies.Where(x => x.PolicyId == id).ExecuteDeleteAsync();
            await context.IdentityTypesPolicies.Where(x => x.PolicyId == id).ExecuteDeleteAsync();
            await context.TimeAccessPolicies.Where(x => x.PolicyId == id).ExecuteDeleteAsync();
            await context.Policies.Where(x => x.Id == id).ExecuteDeleteAsync();

            return new(Responses.Success);
        }
        catch (Exception)
        {
        }
        return new();
    }


    /// <summary>
    /// Obtener una política de acceso por id.
    /// </summary>
    /// <param name="id">Id de la política.</param>
    public async Task<ReadOneResponse<PolicyModel>> Read(int id, bool includeDetails)
    {
        try
        {
            var model = await (from p in context.Policies
                               where p.Id == id
                               select p)
                               .IncludeIf(includeDetails, t => t.Include(t => t.TimeAccessPolicies))
                               .IncludeIf(includeDetails, t => t.Include(t => t.IpAccessPolicies))
                               .IncludeIf(includeDetails, t => t.Include(t => t.IdentityTypePolicies))
                               .FirstOrDefaultAsync();

            if (model is null)
                return new(Responses.NotRows);

            return new(Responses.Success, model);
        }
        catch (Exception)
        {
        }
        return new();
    }

}