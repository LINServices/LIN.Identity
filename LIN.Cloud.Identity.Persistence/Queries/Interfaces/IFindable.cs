namespace LIN.Cloud.Identity.Persistence.Queries.Interfaces;

public interface IFindable<T>
{

    /// <summary>
    /// Encontrar por Id.
    /// </summary>
    /// <param name="id">Id único.</param>
    public IQueryable<T> GetAccounts(int id, Models.QueryObjectFilter filter);

    /// <summary>
    /// Encontrar por unique.
    /// </summary>
    public IQueryable<T> GetAccounts(string unique, Models.QueryObjectFilter filter);

    /// <summary>
    /// Encontrar por Ids.
    /// </summary>
    /// <param name="ids">Lista de ids únicos.</param>
    public IQueryable<T> GetAccounts(List<int> ids, Models.QueryObjectFilter filter);

}