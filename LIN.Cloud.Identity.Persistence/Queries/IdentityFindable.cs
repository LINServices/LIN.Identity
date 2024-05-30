using LIN.Cloud.Identity.Persistence.Queries.Interfaces;
using LIN.Types.Cloud.Identity.Models;

namespace LIN.Cloud.Identity.Persistence.Queries;


public class IdentityFindable : IFindable<AccountModel>
{
    public IQueryable<AccountModel> GetAccounts(int id, Models.QueryObjectFilter filter)
    {
        throw new NotImplementedException();
    }

    public IQueryable<AccountModel> GetAccounts(List<int> ids, Models.QueryObjectFilter filter)
    {
        throw new NotImplementedException();
    }
}