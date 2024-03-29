namespace LIN.Cloud.Identity.Data;


public partial class Accounts
{



    /// <summary>
    /// Crear nueva cuenta. [Transacción]
    /// </summary>
    /// <param name="modelo">Modelo de la cuenta.</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<ReadOneResponse<AccountModel>> Create(AccountModel modelo, DataContext context, int organization = 0)
    {

        // Pre.
        modelo.Id = 0;
        modelo.IdentityId = 0;

        // Transacción.
        using var transaction = context.Database.BeginTransaction();

        try
        {

            // Guardar la cuenta.
            await context.Accounts.AddAsync(modelo);
            context.SaveChanges();

            // Si la organización existe.
            if (organization > 0)
            {

                var generalGroup = (from org in context.Organizations
                                    where org.Id == organization
                                    select org.DirectoryId).FirstOrDefault();

                if (generalGroup <= 0)
                {
                    throw new Exception("Organización no encontrada.");
                }

                var x = new GroupMember()
                {
                    Group = new()
                    {
                        Id = generalGroup
                    },
                    Identity = modelo.Identity,
                    Type = GroupMemberTypes.User
                };

                context.Attach(x.Group);

                context.GroupMembers.Add(x);

                context.SaveChanges();

            }


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
                Response = Responses.ExistAccount
            };
        }

    }



    /// <summary>
    /// Obtener una cuenta según el Id.
    /// </summary>
    /// <param name="id">Id de la cuenta.</param>
    /// <param name="filters">Filtros de búsqueda.</param>
    /// <param name="context">Contexto de base de datos.</param>
    public static async Task<ReadOneResponse<AccountModel>> Read(int id, QueryAccountFilter filters, DataContext context)
    {

        try
        {

            // Consulta de las cuentas.
            var account = await Builders.Account.GetAccounts(id, filters, context).FirstOrDefaultAsync();

            // Si la cuenta no existe.
            if (account == null)
                return new()
                {
                    Response = Responses.NotRows
                };

            // Success.
            return new()
            {
                Response = Responses.Success,
                Model = account
            };

        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.ExistAccount
            };
        }

    }



    /// <summary>
    /// Obtener una cuenta según el identificador único.
    /// </summary>
    /// <param name="unique">Único.</param>
    /// <param name="filters">Filtros de búsqueda.</param>
    /// <param name="context">Contexto de base de datos.</param>
    public static async Task<ReadOneResponse<AccountModel>> Read(string unique, QueryAccountFilter filters, DataContext context)
    {

        try
        {

            // Consulta de las cuentas.
            var account = await Builders.Account.GetAccounts(unique, filters, context).FirstOrDefaultAsync();

            // Si la cuenta no existe.
            if (account == null)
                return new()
                {
                    Response = Responses.NotRows
                };

            // Success.
            return new()
            {
                Response = Responses.Success,
                Model = account
            };

        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.ExistAccount
            };
        }

    }



    /// <summary>
    /// Obtener una cuenta según el id de la identidad.
    /// </summary>
    /// <param name="id">Id de la identidad.</param>
    /// <param name="filters">Filtros de búsqueda.</param>
    /// <param name="context">Contexto de base de datos.</param>
    public static async Task<ReadOneResponse<AccountModel>> ReadByIdentity(int id, QueryAccountFilter filters, DataContext context)
    {

        try
        {

            // Consulta de las cuentas.
            var account = await Builders.Account.GetAccountsByIdentity(id, filters, context).FirstOrDefaultAsync();

            // Si la cuenta no existe.
            if (account == null)
                return new()
                {
                    Response = Responses.NotRows
                };

            // Success.
            return new()
            {
                Response = Responses.Success,
                Model = account
            };

        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.ExistAccount
            };
        }

    }



    /// <summary>
    /// Buscar por patron.
    /// </summary>
    /// <param name="pattern">patron de búsqueda</param>
    /// <param name="filters">Filtros</param>
    /// <param name="context">Contexto de conexión</param>
    public static async Task<ReadAllResponse<AccountModel>> Search(string pattern, QueryAccountFilter filters, DataContext context)
    {

        // Ejecución
        try
        {

            List<AccountModel> accountModels = await Builders.Account.Search(pattern, filters, context).Take(10).ToListAsync();

            // Si no existe el modelo
            if (accountModels == null)
                return new(Responses.NotRows);

            return new(Responses.Success, accountModels);
        }
        catch
        {
        }

        return new();
    }



    /// <summary>
    /// Obtiene los usuarios con IDs coincidentes
    /// </summary>
    /// <param name="ids">Lista de IDs</param>
    /// <param name="context">Contexto de base de datos</param>
    public static async Task<ReadAllResponse<AccountModel>> FindAll(List<int> ids, QueryAccountFilter filters, DataContext context)
    {

        // Ejecución
        try
        {

            var query = Builders.Account.FindAll(ids, filters, context);

            // Ejecuta
            var result = await query.ToListAsync();

            // Si no existe el modelo
            if (result == null)
                return new(Responses.NotRows);

            return new(Responses.Success, result);
        }
        catch
        {
        }

        return new();
    }



}