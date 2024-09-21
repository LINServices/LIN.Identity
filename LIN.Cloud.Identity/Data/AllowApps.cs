namespace LIN.Cloud.Identity.Data;


public class AllowApps(DataContext context, LIN.Cloud.Identity.Services.Utils.IIdentityService identityService)
{


    /// <summary>
    /// Crear acceso a app.
    /// </summary>
    /// <param name="modelo">Modelo.</param>
    public async Task<ReadOneResponse<AllowApp>> Create(AllowApp modelo)
    {

        // Transacción.
        using var transaction = context.Database.BeginTransaction();

        try
        {

            // Attach.
            context.Attach(modelo.Application);
            context.Attach(modelo.Identity);

            // Guardar la cuenta.
            await context.AllowApps.AddAsync(modelo);
            context.SaveChanges();

            // Confirmar los cambios.
            transaction.Commit();

            return new()
            {
                Response = Responses.Success,
                Model = modelo
            };

        }
        catch (Exception)
        {
            transaction.Rollback();
            return new()
            {
                Response = Responses.ResourceExist
            };
        }

    }



    /// <summary>
    /// Obtener las apps a las que una identidad tiene acceso o no.
    /// </summary>
    public async Task<ReadAllResponse<AllowApp>> ReadAll(int id)
    {

        // Ejecución
        try
        {

            var identities = await identityService.GetIdentities(id);

            var query = await (from allow in context.AllowApps
                               where identities.Contains(allow.IdentityId)
                               select new AllowApp
                               {
                                   ApplicationId = allow.ApplicationId,
                                   IdentityId = allow.IdentityId,
                                   IsAllow = allow.IsAllow,
                                   Application = new()
                                   {
                                       Id = allow.Application.Id,
                                       IdentityId = allow.Application.IdentityId,
                                       Name = allow.Application.Name,
                                       OwnerId = allow.Application.OwnerId,
                                   }
                               }).ToListAsync();

            return new(Responses.Success, query);

        }
        catch (Exception)
        {
        }
        return new();
    }


}