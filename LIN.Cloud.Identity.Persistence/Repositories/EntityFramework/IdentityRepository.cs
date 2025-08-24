namespace LIN.Cloud.Identity.Persistence.Repositories.EntityFramework;

internal class IdentityRepository(DataContext context) : IIdentityRepository
{

    /// <summary>
    /// Crear nueva identidad.
    /// </summary>
    public async Task<ReadOneResponse<IdentityModel>> Create(IdentityModel modelo)
    {
        modelo.Id = 0;
        try
        {
            foreach (var rol in modelo.Roles)
                rol.Identity = modelo;

            // Organización propietaria.
            if (modelo.Owner is not null)
                modelo.Owner = context.AttachOrUpdate(modelo.Owner);

            // Guardar la identidad.
            await context.Identities.AddAsync(modelo);
            context.SaveChanges();

            return new(Responses.Success, modelo);
        }
        catch (Exception)
        {
            return new(Responses.ResourceExist);
        }
    }


    /// <summary>
    /// Obtener una identidad según el Id.
    /// </summary>
    /// <param name="id">Id de la identidad.</param>
    /// <param name="filters">Filtros.</param>
    public async Task<ReadOneResponse<IdentityModel>> Read(int id, QueryIdentityFilter filters)
    {
        try
        {
            // Consulta de las cuentas.
            var identity = await Builders.Identities.GetIds(id, filters, context).FirstOrDefaultAsync();

            // Si la cuenta no existe.
            if (identity == null)
                return new(Responses.NotRows);

            // Success.
            return new(Responses.Success, identity);
        }
        catch (Exception)
        {
            return new(Responses.ExistAccount);
        }

    }


    /// <summary>
    /// Validar si existe una identidad según el Unique.
    /// </summary>
    public async Task<ReadOneResponse<bool>> Exist(string unique)
    {
        try
        {
            bool exist = await context.Identities.AnyAsync(x => x.Unique == unique);
            return new(Responses.Success, exist);
        }
        catch (Exception)
        {
            return new(Responses.Undefined);
        }
    }


    /// <summary>
    /// Obtener una identidad según el Unique.
    /// </summary>
    /// <param name="unique">Unique.</param>
    /// <param name="filters">Filtros de búsqueda.</param>
    public async Task<ReadOneResponse<IdentityModel>> Read(string unique, QueryIdentityFilter filters)
    {
        try
        {
            // Consulta de las cuentas.
            var identity = await Builders.Identities.GetIds(unique, filters, context).FirstOrDefaultAsync();

            // Si la cuenta no existe.
            if (identity == null)
                return new(Responses.NotRows);

            // Success.
            return new(Responses.Success, identity);
        }
        catch (Exception)
        {
            return new(Responses.ExistAccount);
        }
    }

}