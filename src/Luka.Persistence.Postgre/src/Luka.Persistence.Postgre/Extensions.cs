using System.Reflection;
using System.Text.RegularExpressions;
using Luka.Attributes;
using Luka.Persistence.Postgre.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Luka.Persistence.Postgre;

public static class Extensions
{
    public static ILukaBuilder AddPostgreSql<TContext>(this ILukaBuilder builder) where TContext : ApplicationDbContext
    {
        var sqlOptions = builder.GetOptions<PostgreSqlOptions>("postgreSQL");

        var connectionString =
            $"Host={sqlOptions.Host};Port={sqlOptions.Port};Database={sqlOptions.Database};Username={sqlOptions.User};Password={sqlOptions.Password}";

        builder.Services.AddDbContext<TContext>(options =>
            options.UseNpgsql(connectionString));

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        builder.Services.AddScoped<IApplicationDbContext, TContext>();
        if(sqlOptions.IsNeedToSyncPermissionEnumToDb)
            builder.SyncPermissionsWithDatabase<TContext>();
        return builder;
    }

    public static IApplicationBuilder UsePostgreSql<TContext>(this IApplicationBuilder app) where TContext : DbContext
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
            // dbContext.Database.EnsureCreated();
            dbContext.Database.Migrate();
            var initializer = scope.ServiceProvider.GetService<IDbInitializer>();
            if (initializer is not null)
            {
                initializer.InitAsync().GetAwaiter().GetResult();
            }
        }


        return app;
    }
    public static ILukaBuilder AddRepository(this ILukaBuilder builder)
    {
        builder.Services.AddTransient(typeof(IRepository<,>), typeof(Repository<,>));
        return builder;
    }
    public static ModelBuilder UseSnakeCaseNamingConvention(this ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Convert C# property names to snake_case for PostgreSQL columns
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.Name.ToSnakeCase());
            }
        }
        return modelBuilder;
    }

    private static string ToSnakeCase(this string str)
        => Regex.Replace(str, "(?<=.)([A-Z])", "_$1").ToLower();

    public static void SyncPermissionsWithDatabase<TContext>(this ILukaBuilder builder)
        where TContext : DbContext, IApplicationDbContext
    {
        using var scope = builder.Services.BuildServiceProvider().CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();

        var enumPermissions = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsEnum && t.GetCustomAttribute<PermissionAttribute>() != null)
            .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static)
                             .Select(f => f.Name))
            .Distinct()
            .ToHashSet();

        var dbSet = dbContext.Context.GetType()
            .GetProperties()
            .FirstOrDefault(p =>
                p.PropertyType.IsGenericType &&
                p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                string.Equals(p.Name, "Permissions", StringComparison.OrdinalIgnoreCase));

        if (dbSet is null)
        {
            throw new InvalidOperationException("DbContext must expose a DbSet named 'Permissions'.");
        }

        var entityType = dbSet.PropertyType.GenericTypeArguments[0];
        var dbPermissionsQueryable = (IQueryable<object>)dbSet.GetValue(dbContext.Context);

        var dbPermissionNames = dbPermissionsQueryable
            .Select(p => EF.Property<string>(p, "PermissionName"))
            .ToHashSet();

        var missing = enumPermissions
            .Except(dbPermissionNames)
            .Select(p =>
            {
                var entity = Activator.CreateInstance(entityType);
                entityType.GetProperty("PermissionName")?.SetValue(entity, p);
                return entity;
            })
            .ToList();

        if (missing.Any())
        {
            var addRangeMethod = typeof(DbContext).GetMethod(
                nameof(DbContext.AddRange),
                new[] { typeof(IEnumerable<object>) });

            addRangeMethod?.Invoke(dbContext.Context, new object[] { missing });
            dbContext.Context.SaveChanges();
        }
    }

}
