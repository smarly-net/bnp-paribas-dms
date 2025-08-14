
using DMS.Infrastructure.Read.Entities;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Reflection.Emit;

namespace DMS.Infrastructure.Read;

public sealed class ReadDbContext : DbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options) { }

    public DbSet<DocumentAccessInviteReadEntity> DocumentAccessInvites => Set<DocumentAccessInviteReadEntity>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        #region Schema

        b.Entity<DocumentAccessInviteReadEntity>(e =>
        {
            e.ToTable("DocumentAccessInvites");
            e.HasKey(x => x.Id);

            e.Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(200);

            e.HasIndex(x => x.Token).IsUnique();
            e.HasIndex(x => x.UserId);
            e.HasIndex(x => x.DocumentId);
            e.Property(x => x.ExpiresAtUtc);
            e.HasIndex(x => new { x.UserId, x.DocumentId, x.ExpiresAtUtc, x.Token });
        });

        #endregion


    }
}