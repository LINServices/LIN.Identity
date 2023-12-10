namespace LIN.Identity.Data;


public class Context : DbContext
{


    /// <summary>
    /// Identidades.
    /// </summary>
    public DbSet<IdentityModel> Identities { get; set; }


    /// <summary>
    /// Cuentas de usuario.
    /// </summary>
    public DbSet<AccountModel> Accounts { get; set; }


    /// <summary>
    /// Organizaciones.
    /// </summary>
    public DbSet<OrganizationModel> Organizations { get; set; }


    /// <summary>
    /// Directorios.
    /// </summary>
    public DbSet<DirectoryModel> Directories { get; set; }


    /// <summary>
    /// Integrantes de Directorios.
    /// </summary>
    public DbSet<DirectoryMember> DirectoryMembers { get; set; }


    /// <summary>
    /// Accesos a organizaciones.
    /// </summary>
    public DbSet<OrganizationAccessModel> OrganizationAccess { get; set; }


    /// <summary>
    /// Tabla de aplicaciones
    /// </summary>
    public DbSet<ApplicationModel> Applications { get; set; }



    /// <summary>
    /// Tabla de registros de login
    /// </summary>
    public DbSet<LoginLogModel> LoginLogs { get; set; }



    /// <summary>
    /// Tabla de correos
    /// </summary>
    public DbSet<EmailModel> Emails { get; set; }


    /// <summary>
    /// Políticas.
    /// </summary>
    public DbSet<PolicyModel> Policies { get; set; }




    /// <summary>
    /// Nuevo contexto a la base de datos
    /// </summary>
    public Context(DbContextOptions<Context> options) : base(options) { }




    /// <summary>
    /// Naming DB
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // Indices y identidad.
        modelBuilder.Entity<IdentityModel>()
           .HasIndex(e => e.Unique)
           .IsUnique();

        // Indices y identidad.
        modelBuilder.Entity<OrganizationModel>()
           .HasIndex(e => e.Domain)
           .IsUnique();

        // Indices y identidad.
        modelBuilder.Entity<ApplicationModel>()
           .HasIndex(e => e.Key)
           .IsUnique();

        // Indices y identidad.
        modelBuilder.Entity<ApplicationModel>()
           .HasIndex(e => e.ApplicationUid)
           .IsUnique();

        // Indices y identidad.
        modelBuilder.Entity<EmailModel>()
           .HasIndex(e => e.Email)
           .IsUnique();


        modelBuilder.Entity<DirectoryMember>()
           .HasOne(p => p.Directory)
           .WithMany()
           .HasForeignKey(p => p.DirectoryId);


        modelBuilder.Entity<ApplicationModel>()
          .HasOne(p => p.Directory)
          .WithMany()
          .HasForeignKey(p => p.DirectoryId);


        modelBuilder.Entity<LoginLogModel>()
       .HasOne(p => p.Application)
       .WithMany()
       .HasForeignKey(p => p.ApplicationID)
       .OnDelete(DeleteBehavior.NoAction);
       ;


        modelBuilder.Entity<LoginLogModel>()
 .HasOne(p => p.Account)
 .WithMany()
 .HasForeignKey(p => p.AccountID);




        modelBuilder.Entity<OrganizationAccessModel>()
            .HasOne(oa => oa.Organization)
            .WithMany(o => o.Members)
            .HasForeignKey(oa => oa.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<DirectoryMember>()
          .HasOne(dm => dm.Directory)
          .WithMany(d => d.Members)
          .HasForeignKey(dm => dm.DirectoryId)
          .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<DirectoryMember>()
         .HasOne(p => p.Account)
         .WithMany(d => d.DirectoryMembers)
         .HasForeignKey(p => p.AccountId)
          .OnDelete(DeleteBehavior.Restrict); ;



        modelBuilder.Entity<PolicyModel>()
         .HasOne(p => p.Directory)
         .WithMany(d => d.Policies)
         .HasForeignKey(p => p.DirectoryId)
          .OnDelete(DeleteBehavior.Restrict); 







        // Configure OrganizationAccessModel
        modelBuilder.Entity<OrganizationAccessModel>()
            .HasOne(o => o.Member)
            .WithOne(a => a.OrganizationAccess)
            .HasForeignKey<OrganizationAccessModel>(o => o.MemberId)
            .IsRequired();

        modelBuilder.Entity<DirectoryMember>()
         .HasKey(t => new
         {
             t.AccountId,
             t.DirectoryId
         });

        modelBuilder.Entity<OrganizationAccessModel>()
         .HasKey(t => new
         {
             t.MemberId,
             t.OrganizationId
         });


        // Nombre de la identidades.
        modelBuilder.Entity<IdentityModel>().ToTable("IDENTITIES");
        modelBuilder.Entity<OrganizationModel>().ToTable("ORGANIZATIONS");
        modelBuilder.Entity<AccountModel>().ToTable("ACCOUNTS");
        modelBuilder.Entity<ApplicationModel>().ToTable("APPLICATIONS");
        modelBuilder.Entity<EmailModel>().ToTable("EMAILS");
        modelBuilder.Entity<DirectoryModel>().ToTable("DIRECTORIES");
        modelBuilder.Entity<DirectoryMember>().ToTable("DIRECTORY_MEMBERS");
        modelBuilder.Entity<OrganizationAccessModel>().ToTable("ORGANIZATIONS_MEMBERS");
        modelBuilder.Entity<LoginLogModel>().ToTable("LOGS");
        modelBuilder.Entity<PolicyModel>().ToTable("POLICIES");

    }


}