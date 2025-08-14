using System.Collections.Generic;

using DMS.Infrastructure.Read.Entities;

using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Read;

public sealed class ReadDbContext : DbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options) { }
    public DbSet<UserReadEntity> Users => Set<UserReadEntity>();
    public DbSet<RoleReadEntity> Roles => Set<RoleReadEntity>();
    public DbSet<UserRoleReadEntity> UserRoles => Set<UserRoleReadEntity>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        #region Schema

        // Users
        b.Entity<UserReadEntity>(e =>
        {
            e.ToTable("Users");
            e.HasKey(x => x.Id);
            e.Property(x => x.Username).HasMaxLength(128).IsRequired();
            e.Property(x => x.PasswordHash).HasMaxLength(256).IsRequired();
            e.HasIndex(x => x.Username).IsUnique();
        });

        // Roles
        b.Entity<RoleReadEntity>(e =>
        {
            e.ToTable("Roles");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(64).IsRequired();
            e.HasIndex(x => x.Name).IsUnique();
        });

        // UserRoles 
        b.Entity<UserRoleReadEntity>(e =>
        {
            e.ToTable("UserRoles");
            e.HasKey(x => new { x.UserId, x.RoleId });

            e.HasOne(x => x.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        #endregion

        #region Seeds

        b.Entity<UserReadEntity>(e =>
        {
            e.HasData(new UserReadEntity
            {
                Id = Guid.NewGuid(),
                Username = "dmitriev.denis",
                PasswordHash = "PLACEHOLDER_HASH"
            });
        });

        #endregion

    }
}