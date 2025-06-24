using Luka.Persistence.Postgre;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Lukachi.Services.Users.Entities;

public class UsersDbContext : ApplicationDbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
    {
        
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbContext Context => this;
    
    protected override void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RolePermission>(e =>
        {
            e.HasKey(x => new { x.RoleId, x.PermissionId });
        });
        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId);
        
        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId);
        
        modelBuilder.Entity<UserRole>(e =>
        {
            e.HasKey(x => new { x.UserId, x.RoleId });
        });
        modelBuilder.Entity<UserRole>()
            .HasOne(rp => rp.User)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(rp => rp.UserId);
        
        modelBuilder.Entity<UserRole>()
            .HasOne(rp => rp.Role)
            .WithMany(p => p.UserRoles)
            .HasForeignKey(rp => rp.RoleId);

        modelBuilder.Entity<Permission>().HasData(new Permission {Id = 1, PermissionName = "CREATE_USER"},
            new Permission {Id = 2, PermissionName = "GET_USER"});
        modelBuilder.Entity<Role>().HasData(new Role {Id = 1, Name = "admin"}, new Role() {Id = 2, Name = "moderator"});
        modelBuilder.Entity<RolePermission>().HasData(new RolePermission() {RoleId = 1, PermissionId = 1},
            new RolePermission() {RoleId = 2, PermissionId = 2});
        modelBuilder.Entity<User>().HasData(new User() {Id = 1, Username = "admin", CreatedAt = DateTime.Now},
            new User() {Id = 2, Username = "moderator", CreatedAt = DateTime.Now});
        modelBuilder.Entity<UserRole>()
            .HasData(new UserRole() {UserId = 1, RoleId = 1}, new UserRole() {UserId = 2, RoleId = 2});
    }
}