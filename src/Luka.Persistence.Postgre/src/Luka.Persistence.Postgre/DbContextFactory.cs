using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Luka.Persistence.Postgre;

public class DbContextFactory<TContext> : IDesignTimeDbContextFactory<TContext> where TContext : ApplicationDbContext
{
    public TContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .Build();
        var sqlOptions = config.GetSection("postgreSQL").Get<PostgreSqlOptions>();
        
        var connectionString =
            $"Host={sqlOptions.Host};Port={sqlOptions.Port};Database={sqlOptions.Database};Username={sqlOptions.User};Password={sqlOptions.Password}";

        var options = new DbContextOptionsBuilder<TContext>()
            .UseNpgsql(connectionString)
            .Options;

        return (TContext)Activator.CreateInstance(typeof(TContext), options)!;
    }
}