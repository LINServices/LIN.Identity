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
    /// Aplicaciones.
    /// </summary>
    public DbSet<ApplicationModel> Applications { get; set; }


    /// <summary>
    /// Allow apps.
    /// </summary>
    public DbSet<AllowApp> AllowApps { get; set; }


    /// <summary>
    /// Logs de accounts.
    /// </summary>
    public DbSet<AccountLog> AccountLogs { get; set; }


    /// <summary>
    /// Políticas.
    /// </summary>
    public DbSet<PolicyModel> Policies { get; set; }


    /// <summary>
    /// Identidades en Políticas.
    /// </summary>
    public DbSet<IdentityAllowedOnPolicyModel> IdentityOnPolicies { get; set; }


    /// <summary>
    /// Requerimientos de políticas.    
    /// </summary>
    public DbSet<PolicyRequirementModel> PolicyRequirements { get; set; }


    /// <summary>
    /// Crear el modelo en BD.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Identity Model
        modelBuilder.Entity<IdentityModel>(entity =>
        {
            entity.ToTable("identities");
            entity.HasIndex(t => t.Unique).IsUnique();
        });

        // Account Model
        modelBuilder.Entity<AccountModel>(entity =>
        {
            entity.ToTable("accounts");
            entity.HasIndex(t => t.IdentityId).IsUnique();
        });

        // Organization Model
        modelBuilder.Entity<OrganizationModel>(entity =>
        {
            entity.ToTable("organizations");
            entity.HasOne(o => o.Directory)
                  .WithOne()
                  .HasForeignKey<OrganizationModel>(o => o.DirectoryId)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // Group Model
        modelBuilder.Entity<GroupModel>(entity =>
        {
            entity.ToTable("groups");
            entity.HasOne(g => g.Owner)
                  .WithMany(o => o.OwnedGroups)
                  .HasForeignKey(g => g.OwnerId)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(t => t.Identity)
                  .WithMany()
                  .HasForeignKey(t => t.IdentityId)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // Group Member Model
        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.ToTable("group_members");
            entity.HasKey(t => new { t.IdentityId, t.GroupId });

            entity.HasOne(t => t.Identity)
                  .WithMany()
                  .HasForeignKey(t => t.IdentityId)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(t => t.Group)
                  .WithMany(t => t.Members)
                  .HasForeignKey(t => t.GroupId);
        });

        // Identity Roles Model
        modelBuilder.Entity<IdentityRolesModel>(entity =>
        {
            entity.ToTable("identity_roles");
            entity.HasKey(t => new { t.Rol, t.IdentityId, t.OrganizationId });

            entity.HasOne(t => t.Identity)
                  .WithMany(t => t.Roles)
                  .HasForeignKey(t => t.IdentityId);

            entity.HasOne(t => t.Organization)
                  .WithMany()
                  .HasForeignKey(t => t.OrganizationId);
        });

        // Application Model
        modelBuilder.Entity<ApplicationModel>(entity =>
        {
            entity.ToTable("applications");
            entity.HasIndex(t => t.IdentityId).IsUnique();

            entity.HasOne(t => t.Identity)
                  .WithMany()
                  .HasForeignKey(t => t.IdentityId);

            entity.HasOne(t => t.Owner)
                  .WithMany()
                  .HasForeignKey(t => t.OwnerId)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // Allow Apps Model
        modelBuilder.Entity<AllowApp>(entity =>
        {
            entity.ToTable("allow_apps");
            entity.HasKey(t => new { t.ApplicationId, t.IdentityId });

            entity.HasOne(t => t.Application)
                  .WithMany()
                  .HasForeignKey(t => t.ApplicationId)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(t => t.Identity)
                  .WithMany()
                  .HasForeignKey(t => t.IdentityId)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // Account Logs Model
        modelBuilder.Entity<AccountLog>(entity =>
        {
            entity.ToTable("account_logs");
            entity.HasOne(t => t.Application)
                  .WithMany()
                  .HasForeignKey(t => t.ApplicationId)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(t => t.Account)
                  .WithMany()
                  .HasForeignKey(t => t.AccountId)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // Policy Model
        modelBuilder.Entity<PolicyModel>(entity =>
        {
            entity.ToTable("policies");
            entity.HasOne(t => t.OwnerIdentity)
                  .WithMany()
                  .HasForeignKey(t => t.OwnerIdentityId)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasMany(t => t.ApplyFor)
                  .WithOne()
                  .HasForeignKey(t => t.PolicyId);

            entity.Property(e => e.Id).IsRequired();
        });

        // Identity Allowed on Policy Model
        modelBuilder.Entity<IdentityAllowedOnPolicyModel>(entity =>
        {
            entity.HasKey(t => new { t.PolicyId, t.IdentityId });

            entity.HasOne(t => t.Policy)
                  .WithMany(t => t.ApplyFor)
                  .HasForeignKey(t => t.PolicyId);

            entity.HasOne(t => t.Identity)
                  .WithMany()
                  .HasForeignKey(t => t.IdentityId);
        });

        // Policy Requirement Model
        modelBuilder.Entity<PolicyRequirementModel>(entity =>
        {
            entity.ToTable("policy_requirements");
            entity.HasOne(t => t.Policy)
                  .WithMany()
                  .HasForeignKey(t => t.PolicyId);
        });

        // Base.
        base.OnModelCreating(modelBuilder);
    }

}