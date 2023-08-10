namespace LIN.Auth.Data;


public class Context : DbContext
{


    /// <summary>
    /// Tabla de cuentas
    /// </summary>
    public DbSet<AccountModel> Accounts { get; set; }



    /// <summary>
    /// Tabla de registros de login
    /// </summary>
    public DbSet<LoginLogModel> LoginLogs { get; set; }



    /// <summary>
    /// Tabla de correos
    /// </summary>
    public DbSet<EmailModel> Emails { get; set; }





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
        modelBuilder.Entity<EmailModel>()
           .HasIndex(e => e.Email)
           .IsUnique();

        // Indices
        modelBuilder.Entity<LoginLogModel>().HasIndex(e => e.ID);

        // Nombre de la tablas
        modelBuilder.Entity<AccountModel>().ToTable("ACCOUNTS");
        modelBuilder.Entity<EmailModel>().ToTable("EMAILS");
        modelBuilder.Entity<LoginLogModel>().ToTable("LOGIN_LOGS");

    }


}