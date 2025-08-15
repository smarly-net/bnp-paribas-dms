using DMS.Application.Abstractions.Repositories;
using DMS.Infrastructure.Read.Entities;

using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Read.Repositories;

public sealed class DocumentAccessInviteProjectionRepository : IDocumentAccessInviteProjectionRepository
{
    private readonly ReadDbContext _db;
    public DocumentAccessInviteProjectionRepository(ReadDbContext db) => _db = db;

    public async Task ProjectAsync(Guid inviteId, Guid userId, Guid documentId, string token, DateTime expiresAtUtc, CancellationToken ct)
    {
        var exists = await _db.DocumentAccessInvites
            .AsNoTracking()
            .AnyAsync(x => x.Token == token, ct);
        if (exists)
        {
            return;
        }

        var row = new DocumentAccessInviteReadEntity
        {
            Id = inviteId,
            UserId = userId,
            DocumentId = documentId,
            Token = token,
            ExpiresAtUtc = expiresAtUtc
        };

        _db.DocumentAccessInvites.Add(row);
        await _db.SaveChangesAsync(ct);
    }
}