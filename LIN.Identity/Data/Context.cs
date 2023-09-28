namespace LIN.Identity.Data;


public class Context : DbContext
{


    /// <summary>
    /// Tabla de cuentas
    /// </summary>
    public DbSet<AccountModel> Accounts { get; set; }


    /// <summary>
    /// Tabla de organizaciones
    /// </summary>
    public DbSet<OrganizationModel> Organizations { get; set; }



    /// <summary>
    /// Tabla de accesos a organizaciones
    /// </summary>
    public DbSet<OrganizationAccessModel> OrganizationAccess { get; set; }



    /// <summary>
    /// Tabla de aplicaciones
    /// </summary>
    public DbSet<ApplicationModel> Applications { get; set; }



    /// <summary>
    /// Tabla de aplicaciones
    /// </summary>
    public DbSet<AppOnOrgModel> AppOnOrg { get; set; }


    /// <summary>
    /// Tabla de registros de login
    /// </summary>
    public DbSet<LoginLogModel> LoginLogs { get; set; }



    /// <summary>
    /// Tabla de correos
    /// </summary>
    public DbSet<EmailModel> Emails { get; set; }



    /// <summary>
    /// Tabla de links únicos
    /// </summary>
    public DbSet<UniqueLink> UniqueLinks { get; set; }



    /// <summary>
    /// Tabla de links únicos para email
    /// </summary>
    public DbSet<MailMagicLink> MailMagicLinks { get; set; }






    /// <summary>
    /// Nuevo contexto a la base de datos
    /// </summary>
    public Context(DbContextOptions<Context> options) : base(options) { }



    /// <summary>
    /// Naming DB
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // Indices y identidad
        modelBuilder.Entity<AccountModel>()
           .HasIndex(e => e.Usuario)
           .IsUnique();

        // Indices y identidad
        modelBuilder.Entity<OrganizationModel>()
           .HasIndex(e => e.Domain)
           .IsUnique();

        // Indices y identidad
        modelBuilder.Entity<ApplicationModel>()
           .HasIndex(e => e.Key)
           .IsUnique();

        // Indices y identidad
        modelBuilder.Entity<ApplicationModel>()
           .HasIndex(e => e.ApplicationUid)
           .IsUnique();


        // Indices y identidad
        modelBuilder.Entity<EmailModel>()
           .HasIndex(e => e.Email)
           .IsUnique();

        // Indices y identidad
        modelBuilder.Entity<UniqueLink>()
           .HasIndex(e => e.Key)
           .IsUnique();

        // Indices y identidad
        modelBuilder.Entity<MailMagicLink>()
           .HasIndex(e => e.Key)
           .IsUnique();

        // Indices
        modelBuilder.Entity<LoginLogModel>().HasIndex(e => e.ID);


        modelBuilder.Entity<AccountModel>()
           .HasOne(a => a.OrganizationAccess)
           .WithOne(oa => oa.Member)
           .HasForeignKey<OrganizationAccessModel>(oa => oa.ID);


        modelBuilder.Entity<AppOnOrgModel>()
           .HasKey(a => new { a.AppID, a.OrgID });



        modelBuilder.Entity<AppOnOrgModel>()
           .HasOne(p => p.App)
           .WithMany()
           .HasForeignKey(p => p.AppID);

        modelBuilder.Entity<AppOnOrgModel>()
           .HasOne(p => p.Organization)
           .WithMany()
           .HasForeignKey(p => p.OrgID);



        // Nombre de la tablas
        modelBuilder.Entity<AccountModel>().ToTable("ACCOUNTS");
        modelBuilder.Entity<OrganizationModel>().ToTable("ORGANIZATIONS");
        modelBuilder.Entity<ApplicationModel>().ToTable("APPLICATIONS");
        modelBuilder.Entity<EmailModel>().ToTable("EMAILS");
        modelBuilder.Entity<LoginLogModel>().ToTable("LOGIN_LOGS");
        modelBuilder.Entity<UniqueLink>().ToTable("UNIQUE_LINKS");
        modelBuilder.Entity<MailMagicLink>().ToTable("EMAIL_MAGIC_LINKS");

    }


}