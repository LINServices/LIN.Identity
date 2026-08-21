namespace LIN.Cloud.Identity.Persistence.Repositories.EntityFramework;

internal class AccountRepository(DataContext context, Queries.AccountFindable accountFindable, IGroupMemberRepository groupMemberRepository) : IAccountRepository
{

    /// <summary>
    /// Crear nueva cuenta.
    /// </summary>
    /// <param name="modelo">Modelo de la cuenta.</param>
    /// <param name="organization">Id de la organización.</param>
    public async Task<ReadOneResponse<AccountModel>> Create(AccountModel modelo, int organization)
    {
        modelo.Id = 0;
        modelo.IdentityId = 0;

        using var transaction = context.Database.GetTransaction();

        try
        {
            await context.Accounts.AddAsync(modelo);
            context.SaveChanges();

            if (organization > 0)
            {

                var directory = (from org in context.Organizations
                                 where org.Id == organization
                                 select org.DirectoryId).FirstOrDefault();

                if (directory <= 0)
                    return new(Responses.NotFoundDirectory)
                    {
                        Message = "El directorio no existe"
                    };

                var groupMember = new GroupMember()
                {
                    Group = new()
                    {
                        Id = directory
                    },
                    Identity = modelo.Identity,
                    Type = GroupMemberTypes.User
                };

                await context.Identities.Where(t => t.Id == modelo.Identity.Id)
                                        .ExecuteUpdateAsync(Identities => Identities.SetProperty(t => t.OwnerId, organization));

                await groupMemberRepository.Create([groupMember]);
            }

            transaction?.Commit();

            return new()
            {
                Response = Responses.Success,
                Model = modelo
            };
        }
        catch (Exception)
        {
            transaction?.Rollback();
            return new(Responses.ExistAccount);
        }
    }


    /// <summary>
    /// Obtener una cuenta según el Id.
    /// </summary>
    /// <param name="id">Id de la cuenta.</param>
    /// <param name="filters">Filtros de búsqueda.</param>
    public async Task<ReadOneResponse<AccountModel>> Read(int id, QueryObjectFilter filters)
    {
        try
        {
            var account = await accountFindable.GetAccounts(id, filters).FirstOrDefaultAsync();

            if (account is null)
                return new(Responses.NotRows);

            return new(Responses.Success, account);
        }
        catch (Exception)
        {
            return new();
        }

    }


    /// <summary>
    /// Obtener una cuenta según el identificador único.
    /// </summary>
    /// <param name="unique">Único.</param>
    /// <param name="filters">Filtros de búsqueda.</param>
    public async Task<ReadOneResponse<AccountModel>> Read(string unique, QueryObjectFilter filters)
    {
        try
        {
            var account = await accountFindable.GetAccounts(unique, filters).IncludeIf(filters.IncludeIdentity, t => t.Include(a => a.Identity)).FirstOrDefaultAsync();

            if (account is null)
                return new(Responses.NotRows);

            return new(Responses.Success, account);
        }
        catch (Exception)
        {
            return new();
        }
    }


    /// <summary>
    /// Obtener una cuenta según el id de la identidad.
    /// </summary>
    /// <param name="id">Id de la identidad.</param>
    /// <param name="filters">Filtros de búsqueda.</param>
    public async Task<ReadOneResponse<AccountModel>> ReadByIdentity(int id, QueryObjectFilter filters)
    {
        try
        {
            var account = await Builders.Account.GetAccountsByIdentity(id, filters, context).FirstOrDefaultAsync();

            if (account is null)
                return new(Responses.NotRows);

            return new(Responses.Success, account);
        }
        catch (Exception)
        {
            return new();
        }
    }


    /// <summary>
    /// Buscar por patron.
    /// </summary>
    /// <param name="pattern">patron de búsqueda</param>
    /// <param name="filters">Filtros</param>
    public async Task<ReadAllResponse<AccountModel>> Search(string pattern, QueryObjectFilter filters)
    {
        try
        {
            List<AccountModel> accountModels = await Builders.Account.Search(pattern, filters, context).Take(10).ToListAsync();

            if (accountModels == null || accountModels.Count == 0)
                return new(Responses.NotRows);

            return new(Responses.Success, accountModels);
        }
        catch (Exception)
        {
        }

        return new();
    }


    /// <summary>
    /// Obtiene los usuarios con IDs coincidentes
    /// </summary>
    /// <param name="ids">Lista de IDs</param>
    public async Task<ReadAllResponse<AccountModel>> FindAll(List<int> ids, QueryObjectFilter filters)
    {

        try
        {

            var query = Builders.Account.FindAll(ids, filters, context);

            var result = await query.ToListAsync();

            if (result == null || result.Count == 0)
                return new(Responses.NotRows);

            return new(Responses.Success, result);
        }
        catch (Exception)
        {
        }

        return new();
    }


    /// <summary>
    /// Obtiene los usuarios con IDs coincidentes
    /// </summary>
    /// <param name="ids">Lista de IDs</param>
    public async Task<ReadAllResponse<AccountModel>> FindAllByIdentities(List<int> ids, QueryObjectFilter filters)
    {
        try
        {
            var query = Builders.Account.FindAllByIdentities(ids, filters, context);

            var result = await query.ToListAsync();

            if (result == null || result.Count == 0)
                return new(Responses.NotRows);

            return new(Responses.Success, result);
        }
        catch (Exception)
        {
        }

        return new();
    }


    /// <summary>
    /// Actualizar contraseña de una cuenta.
    /// </summary>
    /// <param name="accountId">Id de la cuenta.</param>
    /// <param name="password">Nueva contraseña.</param>
    public async Task<ResponseBase> UpdatePassword(int accountId, string password)
    {
        try
        {
            var account = await (from a in context.Accounts
                                 where a.Id == accountId
                                 select a).ExecuteUpdateAsync(Accounts => Accounts.SetProperty(t => t.Password, password));

            if (account <= 0)
                return new(Responses.NotExistAccount);

            return new(Responses.Success);
        }
        catch (Exception)
        {
            return new();
        }
    }


    /// <summary>
    /// Actualizar foto de perfil de una cuenta.
    /// </summary>
    /// <param name="accountId">Id de la cuenta.</param>
    /// <param name="profile">URL de la foto de perfil.</param>
    public async Task<ResponseBase> UpdateProfile(int accountId, string profile)
    {
        try
        {
            var affected = await (from a in context.Accounts
                                  where a.Id == accountId
                                  select a).ExecuteUpdateAsync(accounts => accounts.SetProperty(t => t.Profile, profile));

            if (affected <= 0)
                return new(Responses.NotExistAccount);

            return new(Responses.Success);
        }
        catch (Exception)
        {
            return new();
        }
    }

}