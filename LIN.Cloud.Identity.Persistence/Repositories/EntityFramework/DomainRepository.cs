namespace LIN.Cloud.Identity.Persistence.Repositories.EntityFramework;

internal class DomainRepository(DataContext context) : IDomainRepository
{

    /// <summary>
    /// Agregar un dominio.
    /// </summary>
    /// <param name="modelo">Modelo del dominio..</param>
    public async Task<CreateResponse> Create(DomainModel modelo)
    {
        try
        {
            // La organización ya existe.
            modelo.Organization = context.AttachOrUpdate(modelo.Organization);
            await context.Domains.AddAsync(modelo);
            context.SaveChanges();
            return new(Responses.Success, modelo.Id);
        }
        catch (Exception)
        {
            return new();
        }
    }


    /// <summary>
    /// Obtener un dominio por su unique.
    /// </summary>
    /// <param name="id">Id de la organización.</param>
    public async Task<ReadOneResponse<DomainModel>> Read(string unique)
    {
        try
        {
            // Consultar.
            var domain = await (from g in context.Domains
                                where g.Domain == unique
                                select g).FirstOrDefaultAsync();

            // Si la cuenta no existe.
            if (domain is null)
                return new(Responses.NotRows);

            // Success.
            return new(Responses.Success, domain);
        }
        catch (Exception)
        {
            return new(Responses.NotRows);
        }

    }


    /// <summary>
    /// Verificar un dominio por su unique.
    /// </summary>
    public async Task<ResponseBase> Verify(string unique)
    {
        try
        {
            var identityId = await (from g in context.Domains
                                    where g.Domain == unique
                                    select g).ExecuteUpdateAsync(t => t.SetProperty(t => t.IsVerified, true));

            // Success.
            return new(Responses.Success);
        }
        catch (Exception)
        {
            return new();
        }
    }

}