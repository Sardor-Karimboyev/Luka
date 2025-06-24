using Microsoft.EntityFrameworkCore;

namespace Luka.Persistence.Postgre;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
            
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseSnakeCaseNamingConvention();

        OnModelCreatingPartial(modelBuilder);
    }
    
    protected virtual void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        // This can be optionally overridden
    }

    public DbContext Context => this;
}