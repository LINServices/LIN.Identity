namespace LIN.Cloud.Identity.Persistence.Repositories.EntityFramework;

internal class ApplicationRepository(DataContext context) : IApplicationRepository
{

    /// <summary>
    /// Crear una nueva aplicación.
    /// </summary>
    /// <param name="modelo">Modelo.</param>
    public async Task<CreateResponse> Create(ApplicationModel modelo)
    {
        modelo.Id = 0;

        try
        {
            modelo.Owner = context.AttachOrUpdate(modelo.Owner)!;

            await context.Applications.AddAsync(modelo);
            context.SaveChanges();

            return new(Responses.Success, modelo.Id);
        }
        catch (Exception)
        {
            return new(Responses.Undefined);
        }
    }


    /// <summary>
    /// Obtener una aplicación.
    /// </summary>
    /// <param name="key">Key de la app.</param>
    public async Task<ReadOneResponse<ApplicationModel>> Read(string key)
    {
        try
        {

            var application = await (from ar in context.Applications
                                     where ar.Key == Guid.Parse(key)
                                     select ar).FirstOrDefaultAsync();

            return new(application is null ? Responses.NotRows : Responses.Success, application!);

        }
        catch (Exception)
        {
            return new(Responses.Undefined);
        }
    }


    /// <summary>
    /// Obtener una aplicación.
    /// </summary>
    /// <param name="key">Key de la app.</param>
    public async Task<ReadOneResponse<ApplicationModel>> Read(int id)
    {
        try
        {

            var application = await (from ar in context.Applications
                                     where ar.Id == id
                                     select new ApplicationModel
                                     {
                                         Id = ar.Id,
                                         Name = ar.Name,
                                         Identity = ar.Identity
                                     }).FirstOrDefaultAsync();

            return new(application is null ? Responses.NotRows : Responses.Success, application!);

        }
        catch (Exception)
        {
            return new(Responses.Undefined);
        }
    }


    /// <summary>
    /// Validar si existe una app.
    /// </summary>
    /// <param name="key">Key de la app.</param>
    public async Task<ReadOneResponse<bool>> ExistApp(string key)
    {
        try
        {

            var exist = await (from ar in context.Applications
                               where ar.Key == Guid.Parse(key)
                               select ar).AnyAsync();

            return new(Responses.Success, exist);

        }
        catch (Exception)
        {
            return new(Responses.Undefined);
        }
    }

}