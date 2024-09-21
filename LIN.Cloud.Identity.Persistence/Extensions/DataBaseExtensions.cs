using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace LIN.Cloud.Identity.Persistence.Extensions;

public static class DataBaseExtensions
{

    /// <summary>
    /// Obtener transacción.
    /// </summary
    public static IDbContextTransaction? GetTransaction(this DatabaseFacade facade)
    {
        try
        {
            return facade.BeginTransaction();
        }
        catch (Exception)
        {
            return null;
        }
    }

}