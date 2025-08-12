using System.Collections.Generic;
using DMS.Infrastructure.Read.Entities;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Read;

public sealed class ReadDbContext : DbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options) { }
    public DbSet<UserReadEntity> Users => Set<UserReadEntity>();
}