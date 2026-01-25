using MongoDB.EntityFrameworkCore.Extensions;

namespace LIN.Cloud.Identity.Persistence.Contexts;

public class MongoDataContext(DbContextOptions<MongoDataContext> options) : DbContext(options)
{
    public DbSet<AccessPolicyModel> AccessPolicies { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AccessPolicyModel>(entity =>
        {
            entity.ToCollection("access_policies");
        });
    }
}
