
using DMS.Infrastructure.Read.Entities;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Reflection.Emit;

namespace DMS.Infrastructure.Read;

public sealed class ReadDbContext : DbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options) { }

    public DbSet<DocumentAccessInviteReadEntity> DocumentAccessInvites => Set<DocumentAccessInviteReadEntity>();
    public DbSet<DocumentAccessUserRequestReadEntity> DocumentAccessUserRequests => Set<DocumentAccessUserRequestReadEntity>();

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

        b.Entity<DocumentAccessUserRequestReadEntity>(e =>
        {
            e.ToTable("DocumentAccessUserRequests");

            e.HasKey(x => x.InviteId);

            e.HasOne(x => x.Invite)
                .WithMany(i => i.UserRequests)
                .HasForeignKey(x => x.InviteId)
                .OnDelete(DeleteBehavior.Restrict);

            e.Property(x => x.UserId).IsRequired();

            e.Property(x => x.UserName)
                .IsRequired()
                .HasMaxLength(128);

            e.Property(x => x.DocumentId).IsRequired();

            e.Property(x => x.DocumentTitle)
                .IsRequired()
                .HasMaxLength(256);

            e.Property(x => x.Reason)
                .HasMaxLength(1000);

            e.Property(x => x.AccessType)
                .HasConversion<int>()       
                .IsRequired();

            e.Property(x => x.RequestDate)
                .IsRequired();

            e.Property(x => x.DecisionStatus)
                .HasConversion<int>()     
                .IsRequired();

            e.Property(x => x.DecisionUserId);

            e.Property(x => x.DecisionUserName)
                .HasMaxLength(128);

            e.Property(x => x.DecisionComment)
                .HasMaxLength(1000);
            e.Property(x => x.DecisionDate);    

            e.HasIndex(x => x.UserId);
            e.HasIndex(x => x.DocumentId);
            e.HasIndex(x => x.DecisionStatus);
            e.HasIndex(x => new { x.DecisionStatus, x.RequestDate });
            e.HasIndex(x => new { x.UserId, x.DocumentId });
        });

        #endregion


    }
}