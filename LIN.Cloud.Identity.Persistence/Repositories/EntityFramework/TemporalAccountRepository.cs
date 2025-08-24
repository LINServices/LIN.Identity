namespace LIN.Cloud.Identity.Persistence.Repositories.EntityFramework;

internal class TemporalAccountRepository(DataContext context) : ITemporalAccountRepository
{

    /// <summary>
    /// Crear nueva cuenta temporal.
    /// </summary>
    public async Task<CreateResponse> Create(TemporalAccountModel modelo)
    {
        modelo.Id = 0;
        try
        {
            // Guardar la identidad.
            await context.TemporalAccounts.AddAsync(modelo);
            context.SaveChanges();

            return new(Responses.Success, modelo.Id);
        }
        catch (Exception)
        {
            return new(Responses.ResourceExist);
        }
    }


    /// <summary>
    /// Obtener una cuenta temporal por código de verificación.
    /// </summary>
    /// <param name="verificationCode">Código temporal.</param>
    public async Task<ReadOneResponse<TemporalAccountModel>> ReadWithCode(string verificationCode)
    {
        try
        {
            var temporalAccount = await context.TemporalAccounts
                                  .FirstOrDefaultAsync(x => x.VerificationCode == verificationCode);

            // Si la cuenta no existe.
            if (temporalAccount == null)
                return new(Responses.NotRows);

            // Success.
            return new(Responses.Success, temporalAccount);
        }
        catch (Exception)
        {
            return new();
        }
    }


    /// <summary>
    /// Eliminar una cuenta temporal.
    /// </summary>
    /// <param name="id">Id de la cuenta temporal.</param>
    public async Task<ResponseBase> Delete(int id)
    {
        try
        {
            var count = await context.TemporalAccounts.Where(t => t.Id == id).ExecuteDeleteAsync();

            // Success.
            return new(Responses.Success);
        }
        catch (Exception)
        {
            return new();
        }
    }

}