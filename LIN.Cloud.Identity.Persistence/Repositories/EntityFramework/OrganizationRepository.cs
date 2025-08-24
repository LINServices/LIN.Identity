using LIN.Cloud.Identity.Persistence.Formatters;

namespace LIN.Cloud.Identity.Persistence.Repositories.EntityFramework;

internal class OrganizationRepository(DataContext context) : IOrganizationRepository
{

    /// <summary>
    /// Crear nueva organización.
    /// </summary>
    /// <param name="modelo">Modelo de la organización.</param>
    public async Task<CreateResponse> Create(OrganizationModel modelo)
    {
        // Aislar el contexto de la base de datos.
        using var transaction = context.Database.BeginTransaction();

        try
        {
            // Metadata.
            modelo.Directory.Name = "Directorio General";
            modelo.Directory.Description = "Directorio General de la organización";

            modelo.Directory.Identity.Type = IdentityType.Group;
            modelo.Directory.Identity.Owner = null;
            modelo.Directory.Identity.OwnerId = null;
            // Agregar la organización.
            await context.Organizations.AddAsync(modelo);
            context.SaveChanges();
            modelo.Directory.Identity.Owner = modelo;
            context.SaveChanges();

            // Crear la cuenta administrativa.
            var account = new AccountModel()
            {
                Id = 0,
                Visibility = Visibility.Hidden,
                Name = "Admin",
                Password = $"pwd@{DateTime.UtcNow.Year}",
                Identity = new IdentityModel()
                {
                    Provider = IdentityService.LIN,
                    Status = IdentityStatus.Enable,
                    CreationTime = DateTime.UtcNow,
                    EffectiveTime = DateTime.UtcNow,
                    ExpirationTime = DateTime.UtcNow.AddYears(10),
                    Unique = $"admin@{modelo.Directory.Identity.Unique}"
                }
            };

            // Formatear la cuenta.
            account = Account.Process(account);

            await context.Accounts.AddAsync(account);

            context.SaveChanges();

            // IamRoles.
            var rol = new IdentityRolesModel
            {
                Identity = account.Identity,
                Organization = modelo,
                Rol = Roles.Administrator
            };

            await context.IdentityRoles.AddAsync(rol);

            context.SaveChanges();

            modelo.Directory.Identity.Owner = modelo;
            account.Identity.Owner = modelo;
            modelo.Directory.Members.Add(new()
            {
                Group = modelo.Directory,
                Identity = account.Identity,
                Type = GroupMemberTypes.User
            });

            context.SaveChanges();
            transaction.Commit();

            return new(Responses.Success, modelo.Id);

        }
        catch (Exception)
        {
            transaction.Rollback();
            return new();
        }

    }


    /// <summary>
    /// Obtener una organización por su Id.
    /// </summary>
    /// <param name="id">Id de la organización.</param>
    public async Task<ReadOneResponse<OrganizationModel>> Read(int id)
    {
        try
        {
            // Consultar.
            var org = await (from g in context.Organizations
                             where g.Id == id
                             select g).FirstOrDefaultAsync();

            // Si la cuenta no existe.
            if (org is null)
                return new(Responses.NotRows);

            // Success.
            return new(Responses.Success, org);
        }
        catch (Exception)
        {
            return new(Responses.ExistAccount);
        }

    }


    /// <summary>
    /// Obtener el dominio (Identidad principal) de la organización.
    /// </summary>
    /// <param name="id">Id de la organización.</param>
    public async Task<ReadOneResponse<IdentityModel>> GetDomain(int id)
    {
        try
        {
            var org = await (from g in context.Organizations
                             where g.Id == id
                             select g.Directory.Identity).FirstOrDefaultAsync();

            // Si la cuenta no existe.
            if (org is null)
                return new(Responses.NotRows);

            return new(Responses.Success, org);
        }
        catch (Exception)
        {
            return new(Responses.ExistAccount);
        }

    }


    /// <summary>
    /// Obtener el id del directorio (Grupo principal) de la organización.
    /// </summary>
    /// <param name="id">Id de la organización.</param>
    public async Task<ReadOneResponse<int>> ReadDirectory(int id)
    {
        try
        {
            var groupId = await (from g in context.Organizations
                                 where g.Id == id
                                 select g.DirectoryId).FirstOrDefaultAsync();

            // Si la cuenta no existe.
            if (groupId <= 0)
                return new(Responses.NotRows);

            // Success.
            return new(Responses.Success, groupId);
        }
        catch (Exception)
        {
            return new();
        }

    }


    /// <summary>
    /// Obtener id de la identidad del directorio (Grupo principal) de la organización.
    /// </summary>
    /// <param name="id">Id de la organización.</param>
    public async Task<ReadOneResponse<int>> ReadDirectoryIdentity(int id)
    {
        try
        {
            var identityId = await (from g in context.Organizations
                                    where g.Id == id
                                    select g.Directory.IdentityId).FirstOrDefaultAsync();

            // Si la cuenta no existe.
            if (identityId <= 0)
                return new(Responses.NotRows);

            // Success.
            return new(Responses.Success, identityId);
        }
        catch (Exception)
        {
            return new();
        }

    }

}
