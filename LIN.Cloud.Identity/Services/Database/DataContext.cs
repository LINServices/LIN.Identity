namespace LIN.Cloud.Identity.Services.Database;


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





    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        base.OnConfiguring(optionsBuilder);


    }



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

      


        // Nombres de las tablas.
        modelBuilder.Entity<IdentityModel>().ToTable("IDENTITIES");
        modelBuilder.Entity<AccountModel>().ToTable("ACCOUNTS");
        modelBuilder.Entity<GroupModel>().ToTable("GROUPS");
        modelBuilder.Entity<IdentityRolesModel>().ToTable("IDENTITY_ROLES");
        modelBuilder.Entity<GroupMember>().ToTable("GROUPS_MEMBERS");
        modelBuilder.Entity<OrganizationModel>().ToTable("ORGANIZATIONS");

        // Base.
        base.OnModelCreating(modelBuilder);
    }


}