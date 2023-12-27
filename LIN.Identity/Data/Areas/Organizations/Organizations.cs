using Account = LIN.Identity.Validations.Account;

namespace LIN.Identity.Data.Areas.Organizations;


public class Organizations
{


    #region Abstracciones


    /// <summary>
    /// Crea una organización.
    /// </summary>
    /// <param name="data">Modelo</param>
    public static async Task<ReadOneResponse<OrganizationModel>> Create(OrganizationModel data)
    {
        var (context, contextKey) = Conexión.GetOneConnection();
        var response = await Create(data, context);
        context.CloseActions(contextKey);
        return response;
    }



    /// <summary>
    /// Obtiene una organización.
    /// </summary>
    /// <param name="id">ID de la organización</param>
    public static async Task<ReadOneResponse<OrganizationModel>> Read(int id)
    {
        var (context, contextKey) = Conexión.GetOneConnection();

        var res = await Read(id, context);
        context.CloseActions(contextKey);
        return res;
    }



    /// <summary>
    /// Encontrar el directorio de una organización.
    /// </summary>
    /// <param name="identity">Id del integrante directo.</param>
    public static async Task<ReadOneResponse<DirectoryMember>> FindBaseDirectory(int identity)
    {
        var (context, contextKey) = Conexión.GetOneConnection();

        var res = await FindBaseDirectory(identity, context);
        context.CloseActions(contextKey);
        return res;
    }



    /// <summary>
    /// Encontrar una organización según la identidad de un integrante directo.
    /// </summary>
    /// <param name="identity">Id del integrante.</param>
    public static async Task<ReadOneResponse<DirectoryMember>> FindOrganization(int identity)
    {
        var (context, contextKey) = Conexión.GetOneConnection();

        var res = await FindBaseDirectory(identity, context);
        context.CloseActions(contextKey);
        return res;
    }




    #endregion



    /// <summary>
    /// Crear una organización.
    /// </summary>
    /// <param name="data">Modelo</param>
    /// <param name="context">Contexto de conexión</param>
    public static async Task<ReadOneResponse<OrganizationModel>> Create(OrganizationModel data, Conexión context)
    {

        // Modelo.
        data.ID = 0;

        // Ejecución
        using (var transaction = context.DataBase.Database.BeginTransaction())
        {
            try
            {

                // Guardar datos.
                await context.DataBase.Organizations.AddAsync(data);

                // Guardar en BD.
                context.DataBase.SaveChanges();

                // Cuenta de administración.
                AccountModel account = new()
                {
                    Contraseña = "root123",
                    Creación = DateTime.Now,
                    Estado = AccountStatus.Normal,
                    Gender = Genders.Undefined,
                    ID = 0,
                    Identity = new()
                    {
                        Id = 0,
                        Type = IdentityTypes.Account,
                        Unique = $"root@{data.Directory.Identity.Unique}"
                    },
                    Insignia = AccountBadges.None,
                    Nombre = $"Root user {data.Directory.Identity.Unique}",
                    Perfil = [],
                    Rol = AccountRoles.User,
                    Visibilidad = AccountVisibility.Hidden,
                    IdentityId = 0
                };

                // Procesar la cuenta.
                account = Account.Process(account);

                // Guardar la cuenta.
                context.DataBase.Accounts.Add(account);

                // Agregar el miembro.
                data.Directory.Members.Add(new()
                {
                    Directory = data.Directory,
                    Identity = account.Identity,
                    Rol = DirectoryRoles.SuperManager
                });

                // Guardar.
                context.DataBase.SaveChanges(); ;

                // Commit cambios.
                transaction.Commit();

                return new(Responses.Success, data);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                if ((ex.InnerException?.Message.Contains("Violation of UNIQUE KEY constraint") ?? false) || (ex.InnerException?.Message.Contains("duplicate key") ?? false))
                    return new(Responses.Undefined);

            }
        }
        return new();
    }



    /// <summary>
    /// Obtiene una organización.
    /// </summary>
    /// <param name="id">ID de la organización</param>
    /// <param name="context">Contexto de conexión</param>
    public static async Task<ReadOneResponse<OrganizationModel>> Read(int id, Conexión context)
    {

        // Ejecución
        try
        {

            // Query
            var org = await (from E in context.DataBase.Organizations
                             where E.ID == id
                             select new OrganizationModel
                             {
                                 Directory = null!,
                                 DirectoryId = id,
                                 ID = E.ID,
                                 IsPublic = E.IsPublic,
                                 Name = E.Name,
                             }).FirstOrDefaultAsync();

            // Email no existe
            if (org == null)
                return new(Responses.NotRows);

            return new(Responses.Success, org);
        }
        catch
        {
        }

        return new();
    }



    /// <summary>
    /// Encontrar el directorio de una organización.
    /// </summary>
    /// <param name="identity">Id del integrante directo.</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<ReadOneResponse<DirectoryMember>> FindBaseDirectory(int identity, Conexión context)
    {

        // Ejecución
        try
        {

            // Consulta el directorio base.
            var query = await (from org in context.DataBase.Organizations
                               join dir in context.DataBase.Directories
                               on org.DirectoryId equals dir.ID
                               join mem in context.DataBase.DirectoryMembers
                               on dir.ID equals mem.DirectoryId
                               where mem.Rol != DirectoryRoles.Guest
                               && mem.Rol != DirectoryRoles.RoyalGuest
                               where identity == dir.IdentityId
                               select mem).FirstOrDefaultAsync();

            // Email no existe
            if (query == null)
                return new(Responses.NotRows);

            return new(Responses.Success, query);
        }
        catch
        {
        }

        return new();
    }



    /// <summary>
    /// Encontrar una organización según la identidad de un integrante directo.
    /// </summary>
    /// <param name="identity">Id del integrante.</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<ReadOneResponse<OrganizationModel>> FindOrganization(int identity, Conexión context)
    {

        // Ejecución
        try
        {

            // Consulta el directorio base.
            var query = await (from org in context.DataBase.Organizations
                               join dir in context.DataBase.Directories
                               on org.DirectoryId equals dir.ID
                               join mem in context.DataBase.DirectoryMembers
                               on dir.ID equals mem.DirectoryId
                               where identity == dir.IdentityId
                               select org).FirstOrDefaultAsync();

            // Email no existe
            if (query == null)
                return new(Responses.NotRows);

            return new(Responses.Success, query);
        }
        catch
        {
        }

        return new();
    }



}