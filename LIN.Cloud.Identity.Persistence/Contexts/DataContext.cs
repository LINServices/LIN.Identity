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
    /// Logs de accounts.
    /// </summary>
    public DbSet<AccountLog> AccountLogs { get; set; }

    
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
    /// Dominios.
    /// </summary>
    public DbSet<DomainModel> Domains { get; set; }

    /// <summary>
    /// Cuentas temporales.
    /// </summary>
    public DbSet<TemporalAccountModel> TemporalAccounts { get; set; }

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

            entity.HasOne(o => o.Owner)
                  .WithMany()
                  .HasForeignKey(o => o.OwnerId)
                  .OnDelete(DeleteBehavior.NoAction);
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
            entity.HasIndex(t => t.Key).IsUnique();

            entity.HasOne(t => t.Identity)
                  .WithMany()
                  .HasForeignKey(t => t.IdentityId);

            entity.HasOne(t => t.Owner)
                  .WithMany()
                  .HasForeignKey(t => t.OwnerId)
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
        modelBuilder.Entity<TemporalAccountModel>(entity =>
        {
            entity.ToTable("temporal_accounts");
            entity.HasIndex(e => e.VerificationCode).IsUnique();
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


        modelBuilder.Entity<DomainModel>(entity =>
        {
            entity.ToTable("domains");
            entity.HasOne(t => t.Organization)
                  .WithMany()
                  .HasForeignKey(t => t.OrganizationId);

            entity.HasIndex(t => t.Domain).IsUnique();
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
}