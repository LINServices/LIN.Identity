namespace LIN.Cloud.Identity.Data;

public class Policies(DataContext context, Services.Utils.IIdentityService identityService)
{

    /// <summary>
    /// Crear nueva política.
    /// </summary>
    /// <param name="modelo">Modelo.</param>
    /// <returns>Retorna el id.</returns>
    public async Task<CreateResponse> Create(PolicyModel modelo)
    {
        try
        {

            modelo.Id = Guid.NewGuid();

            // Attach.
            context.Attach(modelo.OwnerIdentity);

            foreach (var e in modelo.ApplyFor)
            {
                e.Identity = context.AttachOrUpdate(e.Identity);
                e.Policy = modelo;
            }

            // Guardar la cuenta.
            await context.Policies.AddAsync(modelo);
            context.SaveChanges();

            return new()
            {
                Response = Responses.Success,
                LastUnique = modelo.Id.ToString()
            };

        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.ResourceExist
            };
        }

    }


    /// <summary>
    /// Obtener las políticas donde el usuario es dueño.
    /// </summary>
    /// <param name="id">Id de la identidad.</param>
    /// <returns>Retorna la lista de políticas.</returns>
    public async Task<ReadAllResponse<PolicyModel>> ReadAllOwn(int id)
    {

        // Ejecución
        try
        {

            // Políticas.
            var policies = await (from policy in context.Policies
                                  where policy.OwnerIdentityId == id
                                  select policy).ToListAsync();

            return new(Responses.Success, policies);

        }
        catch (Exception)
        {
        }
        return new();
    }


    /// <summary>
    /// Obtener las políticas asociadas a una organización.
    /// </summary>
    /// <param name="id">Id de la organización.</param>
    public async Task<ReadAllResponse<PolicyModel>> ReadAll(int id)
    {

        // Ejecución
        try
        {

            // Políticas.
            var policies = await (from policy in context.Policies
                                  join gr in context.Groups
                                  on id equals gr.OwnerId
                                  where policy.OwnerIdentityId == gr.IdentityId
                                  select policy).Distinct().ToListAsync();

            return new(Responses.Success, policies);

        }
        catch (Exception)
        {
        }
        return new();
    }


    /// <summary>
    /// Obtener las políticas aplicables a una identidad.
    /// </summary>
    /// <param name="id">Id de la identidad.</param>
    public async Task<ReadAllResponse<PolicyModel>> ApplicablePolicies(int id)
    {

        // Ejecución
        try
        {

            // Obtener las identidades
            var identities = await identityService.GetIdentities(id);

            // Políticas.
            var policies = await (from policy in context.IdentityOnPolicies
                                  where identities.Contains(policy.IdentityId)
                                  select policy.Policy).Distinct().ToListAsync();

            return new(Responses.Success, policies);

        }
        catch (Exception)
        {
        }
        return new();
    }


    /// <summary>
    /// Validar si una identidad y sus padres tienen acceso a una política.
    /// </summary>
    /// <param name="id">Id de la identidad base.</param>
    /// <param name="policyId">Id de la política.</param>
    public async Task<ResponseBase> HasFor(int id, string policyId)
    {

        // Ejecución
        try
        {

            // Convertir el id.
            var policyResult = Guid.TryParse(policyId, out Guid result);

            // Si hubo un error.
            if (!policyResult)
                return new(Responses.InvalidParam);

            // Obtener identidades base.
            var ids = await identityService.GetIdentities(id);

            // Políticas.
            var have = await (from policy in context.Policies
                              where policy.Id == result
                              && policy.ApplyFor.Any(t => ids.Contains(t.IdentityId))
                              select policy).AnyAsync();

            // Respuesta.
            return new(have ? Responses.Success : Responses.Unauthorized);

        }
        catch (Exception)
        {
        }
        return new();
    }


    /// <summary>
    /// Eliminar una política.
    /// </summary>
    /// <param name="policyId">Id de la política.</param>
    public async Task<ResponseBase> Remove(string policyId)
    {

        // Ejecución
        try
        {

            // Convertir el id.
            var policyResult = Guid.TryParse(policyId, out Guid result);

            // Si hubo un error.
            if (!policyResult)
                return new(Responses.InvalidParam);

            // Eliminar vinculos a políticas.
            var deleted = await (from policy in context.IdentityOnPolicies
                                 where policy.PolicyId == result
                                 select policy).ExecuteDeleteAsync();

            // Políticas.
            deleted = await (from policy in context.Policies
                             where policy.Id == result
                             select policy).ExecuteDeleteAsync();

            // Respuesta.
            return new(Responses.Success);

        }
        catch (Exception)
        {
        }
        return new();
    }


    /// <summary>
    /// Agregar una identidad a una política.
    /// </summary>
    /// <param name="id">Id de la identidad base.</param>
    /// <param name="policyId">Id de la política.</param>
    public async Task<ResponseBase> AddMember(int id, string policyId)
    {

        // Ejecución
        try
        {

            // Convertir el id.
            var policyResult = Guid.TryParse(policyId, out Guid result);

            // Si hubo un error.
            if (!policyResult)
                return new(Responses.InvalidParam);

            IdentityAllowedOnPolicyModel allow = new()
            {
                Policy = new()
                {
                    Id = result
                },
                Identity = new()
                {
                    Id = id
                }
            };

            context.Attach(allow.Policy);
            context.Attach(allow.Identity);

            await context.IdentityOnPolicies.AddAsync(allow);

            context.SaveChanges();

            // Respuesta.
            return new(Responses.Success);

        }
        catch (Exception)
        {
        }
        return new();
    }


    /// <summary>
    /// Eliminar una identidad a una política.
    /// </summary>
    /// <param name="id">Id de la identidad base.</param>
    /// <param name="policyId">Id de la política.</param>
    public async Task<ResponseBase> RemoveMember(int id, string policyId)
    {

        // Ejecución
        try
        {

            // Convertir el id.
            var policyResult = Guid.TryParse(policyId, out Guid result);

            // Si hubo un error.
            if (!policyResult)
                return new(Responses.InvalidParam);

            // Eliminar vínculos a políticas.
            var deleted = await (from policy in context.IdentityOnPolicies
                                 where policy.PolicyId == result
                                 && policy.IdentityId == id
                                 select policy).ExecuteDeleteAsync();

            // Respuesta.
            return new(Responses.Success);

        }
        catch (Exception)
        {
        }
        return new();
    }

}