using LIN.Cloud.Identity.Persistence.Contexts;
using LIN.Cloud.Identity.Persistence.Queries.Interfaces;
using LIN.Types.Cloud.Identity.Enumerations;
using LIN.Types.Cloud.Identity.Models;

namespace LIN.Cloud.Identity.Persistence.Queries;


public class AccountFindable(Contexts.DataContext context) : IFindable<AccountModel>
{



    /// <summary>
    /// Buscar en la cuentas estables.
    /// </summary>
    /// <param name="context">Contexto de base de datos.</param>
    private IQueryable<AccountModel> OnStable()
    {

        // Hora actual.
        var now = DateTime.Now;

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
        var now = DateTime.Now;

        // Consulta.
        var query = from account in context.Accounts
                    select account;

        // Retornar.
        return query;
    }




    public IQueryable<AccountModel> GetAccounts(int id, Models.QueryObjectFilter filters)
    {

        // Query general
        IQueryable<AccountModel> accounts;

        if (filters.FindOn == Models.FindOn.StableAccounts)
            accounts = from account in (filters.FindOn == Models.FindOn.StableAccounts) ? OnStable() : OnAll()
                       where account.Id == id
                       select account;

        // Armar el modelo
        accounts = BuildModel(accounts, filters, context);

        // Retorno
        return accounts;

    }


    public IQueryable<AccountModel> GetAccounts(List<int> ids, Models.QueryObjectFilter filter)
    {
        throw new NotImplementedException();
    }
}