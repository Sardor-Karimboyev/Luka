# Luka.Persistence.Postgre

PostgreSQL persistence support for the Luka platform — built on top of Entity Framework Core.

Provides database context setup, repository patterns, migration helpers, and conventions like `snake_case` mapping.

---

## ✨ Features

- ✅ Easy PostgreSQL DbContext configuration via `AddPostgreSql<TContext>()`
- 🐍 Automatic `snake_case` column naming convention
- 📦 Generic repository registration: `IRepository<TEntity, TKey>`
- 🔄 Automatic migration execution on startup
- 📋 Permission syncing helper for `Luka.Auth` integration

---

## ⚙️ Configuration

Add the PostgreSQL connection details to your `appsettings.json`:

```json
{
  "postgreSQL": {
    "host": "localhost",
    "port": 5432,
    "database": "lukadb",
    "user": "postgres",
    "password": "pass",
    "isNeedToSyncPermissionEnumToDb": true
  }
}
```
---
## 🧱 Setup
1. Create a custom `DbContext`
```csharp
public class UsersDbContext : ApplicationDbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }

    protected override void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        // Don't use OnModelCreating use OnModelCreatingPartial instead
        // Define keys, relationships, seed data
    }
}
```
> [!WARNING]
>
> Don't use OnModelCreating use OnModelCreatingPartial instead

Inherit from `ApplicationDbContext` to enable `snake_case` and partial model configuration support.

---
## 🚀 Usage
1. Register persistence in `Program.cs`
```csharp
   builder.Services
   .AddLuka()
   .AddPostgreSql<UsersDbContext>() // Register PostgreSQL DbContext
   .AddRepository();                // Register generic repository
```
2. Apply migrations automatically
```csharp
   app.UsePostgreSql<UsersDbContext>();
```
3. Optional: Sync permissions with database
   If you're using `Luka.Auth`, you can sync permission enums to your database at startup:

If you're added `isNeedToSyncPermissionEnumToDb: true` in `appsettings.json` property automatically adds permissions
`SyncPermissionsWithDatabase`

---
## 🧪 Migrations
Use a `DbContextFactory` to enable CLI tooling:

```csharp
public class UsersDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
{
public UsersDbContext CreateDbContext(string[] args)
{
var options = new DbContextOptionsBuilder<UsersDbContext>()
.UseNpgsql("Host=localhost;Port=5432;Database=lukadb;Username=postgres;Password=pass")
.Options;

        return new UsersDbContext(options);
    }
}
```
Create a migration:

```bash
dotnet ef migrations add Init --project YourProject --startup-project YourProject
```

---
## 🧩 Extension Methods
🔄 `UseSnakeCaseNamingConvention()`
Applies snake_case naming to all entity properties automatically.

🗃️ `AddRepository()`

Registers:

```csharp
IRepository<TEntity, TKey> => Repository<TEntity, TKey>
```
⚙️ `AddPostgreSql<TContext>()`

Loads config, configures Npgsql, registers context.

🚀 `UsePostgreSql<TContext>()`

Applies migrations on app startup.

---
## 📦 Installation
```bash
dotnet add package Luka.Persistence.Postgre
```
---
## 📄 License

MIT

---
## 🧩 Part of Luka Platform
This package is part of the modular Luka .NET ecosystem for building clean, modern microservices.

Related packages:

- `Luka`

- `Luka.Auth`

- `Luka.Persistence.Redis`

- `Luka.Docs.Swagger`
