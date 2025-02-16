namespace LIN.Cloud.Identity.Data;

public class ApplicationRestrictions(DataContext context)
{


    public async Task<CreateResponse> Create(ApplicationModel modelo)
    {
        // Pre.
        modelo.Id = 0;

        try
        {
            // Modelo ya existe.
            modelo.Owner = context.AttachOrUpdate(modelo.Owner);

            // Guardar la identidad.
            await context.Applications.AddAsync(modelo);
            context.SaveChanges();

            return new()
            {
                Response = Responses.Success,
                LastID = modelo.Id
            };
        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.Undefined
            };
        }
    }



    /// <summary>
    /// Crear nueva restriccion de aplicación.
    /// </summary>
    /// <param name="modelo">Modelo.</param>
    public async Task<CreateResponse> Create(ApplicationRestrictionModel modelo)
    {
        // Pre.
        modelo.Id = 0;

        try
        {
            // Modelo ya existe.
            modelo.Application = context.AttachOrUpdate(modelo.Application);

            // Guardar la identidad.
            await context.ApplicationRestrictions.AddAsync(modelo);
            context.SaveChanges();

            return new()
            {
                Response = Responses.Success,
                LastID = modelo.Id
            };
        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.Undefined
            };
        }
    }


    /// <summary>
    /// Obtener las restricciones de aplicacion.
    /// </summary>
    /// <param name="id">Id de la aplicación.</param>
    public async Task<ReadAllResponse<ApplicationRestrictionModel>> ReadAll(int id)
    {

        try
        {

            var restrictions = await (from ar in context.ApplicationRestrictions
                                      where ar.ApplicationId == id
                                      select ar).ToListAsync();

            // Success.
            return new(Responses.Success, restrictions);

        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.ExistAccount
            };
        }
    }


    public async Task<ReadAllResponse<ApplicationRestrictionModel>> ReadAll(string id)
    {

        try
        {

            var restrictions = await (from ar in context.ApplicationRestrictions
                                      where ar.Application.Key == Guid.Parse(id)
                                      select ar).ToListAsync();

            // Success.
            return new(Responses.Success, restrictions);

        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.ExistAccount
            };
        }
    }



    public async Task<ReadOneResponse<ApplicationModel>> Read(string id)
    {

        try
        {

            var restrictions = await (from ar in context.Applications
                                      where ar.Key == Guid.Parse(id)
                                      select ar).FirstOrDefaultAsync();

            // Success.
            return new(restrictions is null ? Responses.NotRows : Responses.Success, restrictions);

        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.ExistAccount
            };
        }
    }


    public async Task<ReadOneResponse<bool>> ExistApp(string id)
    {

        try
        {

            var restrictions = await (from ar in context.Applications
                                      where ar.Key == Guid.Parse(id)
                                      select ar).AnyAsync();

            // Success.
            return new(Responses.Success, restrictions);

        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.Undefined
            };
        }
    }

}