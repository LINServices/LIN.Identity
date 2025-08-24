using LIN.Cloud.Identity.Persistence.Queries.Interfaces;

namespace LIN.Cloud.Identity.Persistence.Queries;

public class AccountFindable(DataContext context) : IFindable<AccountModel>
{

    /// <summary>
    /// Buscar en la cuentas estables.
    /// </summary>
    /// <param name="context">Contexto de base de datos.</param>
    private IQueryable<AccountModel> OnStable()
    {

        // Hora actual.
        var now = DateTime.UtcNow;

        // Consulta.
        var query = from account in context.Accounts
                    where account.Identity.Status != IdentityStatus.Disable
                    && account.Identity.EffectiveTime < now && account.Identity.ExpirationTime > now
                    select account;

        // Retornar.
        return query;
    }


    /// <summary>
    /// Buscar en todas las cuentas.
    /// </summary>
    /// <param name="context">Contexto de base de datos.</param>
    private IQueryable<AccountModel> OnAll()
    {

        // Hora actual.
        var now = DateTime.UtcNow;

        // Consulta.
        var query = from account in context.Accounts
                    select account;

        // Retornar.
        return query;
    }


    /// <summary>
    /// Obtener las cuentas según el Id.
    /// </summary>
    /// <param name="id">Id de la cuenta.</param>
    /// <param name="filters">Filtros.</param>
    public IQueryable<AccountModel> GetAccounts(int id, QueryObjectFilter filters)
    {
        // Query general
        IQueryable<AccountModel> accounts;

        accounts = from account in (filters.FindOn == Models.FindOn.StableAccounts) ? OnStable() : OnAll()
                   where account.Id == id
                   select account;

        // Retorno
        return accounts;
    }

    public IQueryable<AccountModel> GetAccounts(string user, QueryObjectFilter filters)
    {

        // Query general
        IQueryable<AccountModel> accounts;

        accounts = from account in (filters.FindOn == Models.FindOn.StableAccounts) ? OnStable() : OnAll()
                   where account.Identity.Unique == user
                   select account;

        // Retorno
        return accounts;

    }


    public IQueryable<AccountModel> GetAccounts(List<int> ids, Models.QueryObjectFilter filter)
    {
        throw new NotImplementedException();
    }
}