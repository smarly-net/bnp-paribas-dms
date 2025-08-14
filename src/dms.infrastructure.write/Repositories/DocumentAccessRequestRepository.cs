using DMS.Application.Abstractions.Persistence.Write;
using DMS.Application.Abstractions.Repositories;
using DMS.Infrastructure.Write.Entities;

namespace DMS.Infrastructure.Write.Repositories;

public sealed class DocumentAccessRequestRepository : IDocumentAccessRequestRepository
{
    private readonly WriteDbContext _db;

    public DocumentAccessRequestRepository(WriteDbContext db)
    {
        _db = db;
    }

    public async Task IssueAccessInvite(AccessInvite invite, CancellationToken ct)
    {
        var entity = new DocumentAccessRequestEntity
        {
            Id = Guid.NewGuid(),
            UserId = invite.UserId,
            DocumentId = invite.DocumentId,
            RequestToken = invite.Token,
            ExpiredAt = invite.ExpiresAtUtc
        };

        await _db.Set<DocumentAccessRequestEntity>().AddAsync(entity, ct);
    }
}