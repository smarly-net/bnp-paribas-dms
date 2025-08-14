using DMS.Application.Abstractions.Persistence.Read;
using DMS.Application.Abstractions.Repositories;

using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Write.Repositories;

public sealed class DocumentRepository : IDocumentRepository
{
    private readonly WriteDbContext _db;
    public DocumentRepository(WriteDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<DocumentItem>> GetAllAsync(CancellationToken ct)
    {
        var items = await _db.Documents
            .AsNoTracking()
            .Select(d => new DocumentItem(d.Id, d.Title))
            .ToListAsync(ct);

        return items;
    }

    public async Task<DocumentItem?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.Documents
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(d => new DocumentItem(d.Id, d.Title))
            .FirstOrDefaultAsync(ct);
    }
}