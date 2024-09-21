using LIN.Cloud.Identity.Persistence.Models;
using LIN.Types.Cloud.Identity.Models;
using Microsoft.EntityFrameworkCore;

namespace LIN.Cloud.Identity.Persistence.Contexts;


public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{


    /// <summary>
    /// Tabla de identidades.
    /// </summary>
    public DbSet<IdentityModel> Identities { get; set; }


    /// <summary>
    /// Tabla de cuentas.
    /// </summary>
    public DbSet<AccountModel> Accounts { get; set; }


    /// <summary>
    /// Organizaciones.
    /// </summary>
    public DbSet<OrganizationModel> Organizations { get; set; }


    /// <summary>
    /// Grupos.
    /// </summary>
    public DbSet<GroupModel> Groups { get; set; }


    /// <summary>
    /// Integrantes de un grupo.
    /// </summary>
    public DbSet<GroupMember> GroupMembers { get; set; }


    /// <summary>
    /// RolesIam de grupos.
    /// </summary>
    public DbSet<IdentityRolesModel> IdentityRoles { get; set; }


    /// <summary>
    /// PassKeys.
    /// </summary>
    public DbSet<PassKeyDBModel> PassKeys { get; set; }


    /// <summary>
    /// Aplicaciones.
    /// </summary>
    public DbSet<ApplicationModel> Applications { get; set; }



    /// <summary>
    /// Allow apps.
    /// </summary>
    public DbSet<AllowApp> AllowApps { get; set; }


    /// <summary>
    /// Generación del modelo de base de datos.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // Modelo: Identity.
        {
            modelBuilder.Entity<IdentityModel>()
                      .HasIndex(t => t.Unique)
                      .IsUnique();
        }

        // Modelo: Account.
        {
            modelBuilder.Entity<AccountModel>()
                      .HasIndex(t => t.IdentityId)
                      .IsUnique();
        }

        // Modelo: Account.
        {

            modelBuilder.Entity<OrganizationModel>()
                .HasOne(o => o.Directory)
                .WithOne()
                .HasForeignKey<OrganizationModel>(o => o.DirectoryId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<GroupModel>()
                .HasOne(g => g.Owner)
                .WithMany(o => o.OwnedGroups)
                .HasForeignKey(g => g.OwnerId)
                .OnDelete(DeleteBehavior.NoAction);


        }

        // Modelo: PassKey.
        {

            modelBuilder.Entity<PassKeyDBModel>()
                              .HasOne(t => t.Account)
                              .WithMany()
                              .HasForeignKey(y => y.AccountId)
                              .OnDelete(DeleteBehavior.NoAction);
        }

        // Modelo: GroupModel.
        {

            modelBuilder.Entity<GroupModel>()
                .HasOne(t => t.Identity)
                .WithMany()
                .HasForeignKey(t => t.IdentityId)
                .OnDelete(DeleteBehavior.NoAction);


        }

        // Modelo: GroupMemberModel.
        {

            modelBuilder.Entity<GroupMember>()
                              .HasKey(t => new
                              {
                                  t.IdentityId,
                                  t.GroupId
                              });

            modelBuilder.Entity<GroupMember>()
                               .HasOne(t => t.Identity)
                               .WithMany()
                               .HasForeignKey(y => y.IdentityId)
                               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<GroupMember>()
                      .HasOne(t => t.Group)
                      .WithMany(t => t.Members)
                      .HasForeignKey(y => y.GroupId);



        }

        // Modelo: IdentityRolesModel.
        {
            modelBuilder.Entity<IdentityRolesModel>()
                      .HasOne(t => t.Identity)
                      .WithMany(t => t.Roles)
                      .HasForeignKey(y => y.IdentityId);

            modelBuilder.Entity<IdentityRolesModel>()
                     .HasOne(t => t.Organization)
                     .WithMany()
                     .HasForeignKey(y => y.OrganizationId);

            modelBuilder.Entity<IdentityRolesModel>()
                    .HasKey(t => new
                    {
                        t.Rol,
                        t.IdentityId,
                        t.OrganizationId
                    });

        }

        // Modelo: Application.
        {
            modelBuilder.Entity<ApplicationModel>()
                      .HasOne(t => t.Identity)
                      .WithMany()
                      .HasForeignKey(t=>t.IdentityId);


            modelBuilder.Entity<ApplicationModel>()
                  .HasIndex(t => t.IdentityId)
                  .IsUnique();

            modelBuilder.Entity<ApplicationModel>()
                      .HasOne(t => t.Owner)
                      .WithMany()
                      .HasForeignKey(t => t.OwnerId)
                      .OnDelete(DeleteBehavior.NoAction);

        }

        // Modelo: Allow Apps.
        {
            modelBuilder.Entity<AllowApp>()
                      .HasOne(t => t.Application)
                      .WithMany()
                      .HasForeignKey(t => t.ApplicationId)
                      .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AllowApp>()
                      .HasOne(t => t.Identity)
                      .WithMany()
                      .HasForeignKey(t => t.IdentityId)
                      .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AllowApp>()
                           .HasKey(t => new
                           {
                               t.ApplicationId,
                               t.IdentityId
                           });

        }

        // Nombres de las tablas.
        modelBuilder.Entity<IdentityModel>().ToTable("identities");
        modelBuilder.Entity<AccountModel>().ToTable("accounts");
        modelBuilder.Entity<GroupModel>().ToTable("groups");
        modelBuilder.Entity<IdentityRolesModel>().ToTable("identity_roles");
        modelBuilder.Entity<GroupMember>().ToTable("group_members");
        modelBuilder.Entity<OrganizationModel>().ToTable("organizations");
        modelBuilder.Entity<PassKeyDBModel>().ToTable("access_keys");
        modelBuilder.Entity<AllowApp>().ToTable("allow_apps");
        modelBuilder.Entity<ApplicationModel>().ToTable("applications");

        // Base.
        base.OnModelCreating(modelBuilder);
    }


}