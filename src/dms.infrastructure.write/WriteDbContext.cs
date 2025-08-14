using DMS.Infrastructure.Write.Entities;

using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Write;

public sealed class WriteDbContext : DbContext
{
    public WriteDbContext(DbContextOptions<WriteDbContext> options) : base(options) { }
    public DbSet<OutboxMessageEntity> OutboxMessages => Set<OutboxMessageEntity>();

    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();
    public DbSet<RoleEntity> Roles => Set<RoleEntity>();
    public DbSet<UserRoleEntity> UserRoles => Set<UserRoleEntity>();

    public DbSet<DocumentEntity> Documents => Set<DocumentEntity>();
    public DbSet<DocumentAccessRequestEntity> DocumentAccessRequests => Set<DocumentAccessRequestEntity>();
    public DbSet<DocumentRequestDecisionEntity> DocumentRequestDecisions => Set<DocumentRequestDecisionEntity>();


    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        #region Schema

        // Users
        b.Entity<UserEntity>(e =>
        {
            e.ToTable("Users");
            e.HasKey(x => x.Id);
            e.Property(x => x.Username).HasMaxLength(128).IsRequired();
            e.Property(x => x.PasswordHash).HasMaxLength(256).IsRequired();
            e.HasIndex(x => x.Username).IsUnique();
        });

        // Roles
        b.Entity<RoleEntity>(e =>
        {
            e.ToTable("Roles");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(64).IsRequired();
            e.HasIndex(x => x.Name).IsUnique();
        });

        // UserRoles 
        b.Entity<UserRoleEntity>(e =>
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

        //Documents
        b.Entity<DocumentEntity>(e =>
        {
            e.ToTable("Documents");
            e.HasKey(x => x.Id);

            e.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(256);

            e.Property(x => x.Content)
                .IsRequired();
        });

        // RefreshTokenEntity
        b.Entity<RefreshTokenEntity>(e =>
        {
            e.ToTable("RefreshTokens");
            e.HasKey(x => x.Id);

            e.Property(x => x.UserId)
                .IsRequired();

            e.Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(512);

            e.Property(x => x.ExpiryDate)
                .IsRequired();

            e.Property(x => x.ParentAccessJti)
                .IsRequired()
                .HasMaxLength(64);

            e.HasIndex(x => x.Token).IsUnique();
            e.HasIndex(x => x.UserId);
        });

        // OutboxMessageEntity
        b.Entity<OutboxMessageEntity>(e =>
        {
            e.ToTable("OutboxMessages");
            e.HasKey(x => x.Id);

            e.Property(x => x.Type)
                .IsRequired()
                .HasMaxLength(256);

            e.Property(x => x.Payload)
                .IsRequired();

            e.Property(x => x.OccurredAtUtc)
                .IsRequired();

            e.Property(x => x.ProcessedAtUtc);
            e.Property(x => x.Attempts);
            e.Property(x => x.NextTryUtc);

            e.HasIndex(x => new { x.ProcessedAtUtc, x.NextTryUtc });
        });

        //DocumentAccessRequests
        b.Entity<DocumentAccessRequestEntity>(e =>
        {
            e.ToTable("DocumentAccessRequests");
            e.HasKey(x => x.Id);

            e.Property(x => x.UserId).IsRequired();
            e.Property(x => x.DocumentId).IsRequired();

            e.Property(x => x.RequestToken)
                .IsRequired()
                .HasMaxLength(200);

            e.HasIndex(x => x.RequestToken)
                .IsUnique();

            e.Property(x => x.AccessType)
                .HasConversion<int?>();

            e.Property(x => x.AccessReason)
                .HasMaxLength(1000);

            e.Property(x => x.ExpiredAt)
                .IsRequired();

            e.Property(x => x.DocumentId)
                .IsRequired();

            e.HasIndex(x => x.UserId);
            e.HasIndex(x => x.DocumentId);
            e.HasIndex(x => new { x.UserId, x.DocumentId });

        });

        b.Entity<DocumentRequestDecisionEntity>(e =>
        {
            e.ToTable("DocumentRequestDecisions");
            e.HasKey(x => x.Id);

            e.Property(x => x.DecisionStatus)
                .HasConversion<int>()
                .IsRequired();

            e.Property(x => x.Comment)
                .HasMaxLength(1000);

            e.Property(x => x.CreatedAtUtc)
                .IsRequired();

            e.HasOne<DocumentAccessRequestEntity>()
                .WithMany()
                .HasForeignKey(x => x.DocumentAccessRequestId);

            e.HasIndex(x => x.DocumentAccessRequestId);
            e.HasIndex(x => x.DecisionStatus);
            e.HasIndex(x => new { x.DecisionStatus, x.CreatedAtUtc });
        });

        #endregion

        #region Seeds

        Guid dmitrievAdminUserId = Guid.NewGuid();
        Guid adamsApproverUserId = Guid.NewGuid();
        Guid allenUserUserId = Guid.NewGuid();

        b.Entity<UserEntity>(e =>
        {
            e.HasData(new UserEntity
            {
                Id = dmitrievAdminUserId,
                Username = "dmitriev.denis",
                PasswordHash = "PLACEHOLDER_HASH1"
            });

            e.HasData(new UserEntity
            {
                Id = adamsApproverUserId,
                Username = "adams.brown",
                PasswordHash = "PLACEHOLDER_HASH2"
            });

            e.HasData(new UserEntity
            {
                Id = allenUserUserId,
                Username = "allen.smith",
                PasswordHash = "PLACEHOLDER_HASH3"
            });
        });

        Guid adminRoleId = Guid.NewGuid();
        Guid approverRoleId = Guid.NewGuid();

        b.Entity<RoleEntity>(e =>
        {
            e.HasData(new RoleEntity
            {
                Id = adminRoleId,
                Name = "admin",
            });

            e.HasData(new RoleEntity
            {
                Id = approverRoleId,
                Name = "approver"
            });
        });

        b.Entity<UserRoleEntity>(e =>
        {
            e.HasData(new UserRoleEntity
            {
                RoleId = adminRoleId,
                UserId = dmitrievAdminUserId,
            });

            e.HasData(new UserRoleEntity
            {
                RoleId = approverRoleId,
                UserId = adamsApproverUserId,
            });
        });

        b.Entity<DocumentEntity>(e =>
        {
            e.HasData(
                new DocumentEntity
                {
                    Id = Guid.NewGuid(),
                    Title = "Project Plan",
                    Content = "Content of Project Plan"
                },
                new DocumentEntity
                {
                    Id = Guid.NewGuid(),
                    Title = "Internal Guidelines",
                    Content = "Content of Guidelines"
                }
            );
        });

        #endregion

    }
}