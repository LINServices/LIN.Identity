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
                              && policy.ApplyFor.Any(t => ids.Contains(t.Id))
                              select policy).AnyAsync();

            // Respuesta.
            return new(have ? Responses.Success : Responses.Unauthorized);

        }
        catch (Exception)
        {
        }
        return new();
    }

}