using LIN.Cloud.Identity.Persistence.Extensions;
using LIN.Cloud.Identity.Persistence.Models;
using LIN.Types.Cloud.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
    /// Restricciones de apps.
    /// </summary>
    public DbSet<ApplicationRestrictionModel> ApplicationRestrictions { get; set; }


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
    /// Códigos OTPS.
    /// </summary>
    public DbSet<OtpDatabaseModel> OTPs { get; set; }


    /// <summary>
    /// Correos asociados a las cuentas.
    /// </summary>
    public DbSet<MailModel> Mails { get; set; }


    /// <summary>
    /// Mail Otp.
    /// </summary>
    public DbSet<MailOtpDatabaseModel> MailOtp { get; set; }


    /// <summary>
    /// Crear el modelo en BD.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Identity Model.
        modelBuilder.Entity<IdentityModel>(entity =>
        {
            entity.ToTable("identities");
            entity.HasIndex(t => t.Unique).IsUnique();
        });

        // Account Model.
        modelBuilder.Entity<AccountModel>(entity =>
        {
            entity.ToTable("accounts");
            entity.HasIndex(t => t.IdentityId).IsUnique();
        });

        // Organization Model.
        modelBuilder.Entity<OrganizationModel>(entity =>
        {
            entity.ToTable("organizations");
            entity.HasOne(o => o.Directory)
                  .WithOne()
                  .HasForeignKey<OrganizationModel>(o => o.DirectoryId)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // Group Model.
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

        // Application restrictions Model
        modelBuilder.Entity<ApplicationRestrictionModel>(entity =>
        {
            entity.ToTable("applications_restrictions");
            entity.HasOne(t => t.Application)
                  .WithMany(t => t.Restrictions)
                  .HasForeignKey(t => t.ApplicationId)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // Application Model
        modelBuilder.Entity<ApplicationModel>(entity =>
        {
            entity.ToTable("applications");
            entity.HasIndex(t => t.IdentityId).IsUnique();
            entity.HasIndex(t => t.Key).IsUnique();

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

        // Códigos OTPS.
        modelBuilder.Entity<OtpDatabaseModel>(entity =>
        {
            entity.ToTable("otp_codes");
            entity.HasOne(t => t.Account)
                  .WithMany()
                  .HasForeignKey(t => t.AccountId);
        });

        // Correos.
        modelBuilder.Entity<MailModel>(entity =>
        {
            entity.ToTable("mails");
            entity.HasOne(t => t.Account)
                  .WithMany()
                  .HasForeignKey(t => t.AccountId);

            entity.HasIndex(t => t.Mail).IsUnique();
        });

        // Mail OTP.
        modelBuilder.Entity<MailOtpDatabaseModel>(entity =>
        {
            entity.ToTable("mail_otp");

            entity.HasOne(t => t.MailModel)
                  .WithMany()
                  .HasForeignKey(t => t.MailId)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(t => t.OtpDatabaseModel)
                 .WithMany()
                 .HasForeignKey(t => t.OtpId)
                 .OnDelete(DeleteBehavior.NoAction);

            entity.HasKey(t => new { t.MailId, t.OtpId });

        });

        // Base.
        base.OnModelCreating(modelBuilder);
    }


    public void Seed()
    {
        if (!Accounts.Any())
        {
            var jsonData = File.ReadAllText("wwwroot/seeds/users.json");
            var users = JsonConvert.DeserializeObject<List<AccountModel>>(jsonData) ?? [];

            foreach (var user in users)
            {
                user.Password = Global.Utilities.Cryptography.Encrypt(user.Password);
            }

            if (users != null && users.Count > 0)
            {
                Accounts.AddRange(users);
                SaveChanges();
            }
        }

        if (!Applications.Any())
        {
            var jsonData = File.ReadAllText("wwwroot/seeds/applications.json");
            var apps = JsonConvert.DeserializeObject<List<ApplicationModel>>(jsonData) ?? [];

            foreach (var app in apps)
            {
                app.Identity.Type = Types.Cloud.Identity.Enumerations.IdentityType.Application;
                app.Owner = new() { Id = app.OwnerId };
                app.Owner = EntityFramework.AttachOrUpdate(this, app.Owner);
            }

            if (apps != null && apps.Count > 0)
            {
                Applications.AddRange(apps);
                SaveChanges();
            }
        }
    }
}