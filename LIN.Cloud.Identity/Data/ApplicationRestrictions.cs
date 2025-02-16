namespace LIN.Cloud.Identity.Data;

public class ApplicationRestrictions(DataContext context)
{

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
            return new(Responses.Success, modelo.Id);
        }
        catch (Exception)
        {
            return new(Responses.Undefined);
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
            return new(Responses.Undefined);
        }
    }


    /// <summary>
    /// Obtener las restricciones de aplicacion.
    /// </summary>
    /// <param name="id">Id de la aplicación.</param>
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
            return new(Responses.Undefined);
        }
    }

}