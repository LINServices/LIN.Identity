using LIN.Cloud.Identity.Persistence.Queries.Interfaces;

namespace LIN.Cloud.Identity.Persistence.Queries;

public class AccountFindable(DataContext context) : IFindable<AccountModel>
{

    /// <summary>
    /// Buscar en la cuentas estables.
    /// </summary>
    private IQueryable<AccountModel> OnStable()
    {

        var now = DateTime.UtcNow;

        var query = from account in context.Accounts
                    where account.Identity.Status != IdentityStatus.Disable
                    && account.Identity.EffectiveTime < now && account.Identity.ExpirationTime > now
                    select account;

        return query;
    }


    /// <summary>
    /// Buscar en todas las cuentas.
    /// </summary>
    private IQueryable<AccountModel> OnAll()
    {

        var now = DateTime.UtcNow;

        var query = from account in context.Accounts
                    select account;

        return query;
    }


    /// <summary>
    /// Obtener las cuentas según el Id.
    /// </summary>
    /// <param name="id">Id de la cuenta.</param>
    /// <param name="filters">Filtros.</param>
    public IQueryable<AccountModel> GetAccounts(int id, QueryObjectFilter filters)
    {
        IQueryable<AccountModel> accounts;

        accounts = from account in (filters.FindOn == Models.FindOn.StableAccounts) ? OnStable() : OnAll()
                   where account.Id == id
                   select account;

        return accounts;
    }

    public IQueryable<AccountModel> GetAccounts(string user, QueryObjectFilter filters)
    {

        IQueryable<AccountModel> accounts;

        accounts = from account in (filters.FindOn == Models.FindOn.StableAccounts) ? OnStable() : OnAll()
                   where account.Identity.Unique == user
                   select account;

        return accounts;

    }


    public IQueryable<AccountModel> GetAccounts(List<int> ids, Models.QueryObjectFilter filter)
    {
        throw new NotImplementedException();
    }
}