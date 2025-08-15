using DMS.Application.Abstractions.Repositories;

namespace DMS.Infrastructure.Write;

public class UnitOfWork : IUnitOfWork
{
    private readonly WriteDbContext _db;
    public UnitOfWork(WriteDbContext db) => _db = db;
    public Task Commit(CancellationToken ct) => _db.SaveChangesAsync(ct);
}