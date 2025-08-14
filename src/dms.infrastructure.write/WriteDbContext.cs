using DMS.Infrastructure.Write.Entities;

using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Write;

public sealed class WriteDbContext : DbContext
{
    public WriteDbContext(DbContextOptions<WriteDbContext> options) : base(options)
    {
    }

    public DbSet<OutboxMessageEntity> OutboxMessages => Set<OutboxMessageEntity>();
    public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

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
    }
}