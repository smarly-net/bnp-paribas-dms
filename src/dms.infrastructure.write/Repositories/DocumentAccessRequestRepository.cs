using DMS.Application.Abstractions.Persistence.Write;
using DMS.Application.Abstractions.Repositories;
using DMS.Domain.DocumentAccesses;
using DMS.Infrastructure.Write.Entities;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Write.Repositories;

public sealed class DocumentAccessRequestRepository : IDocumentAccessRequestRepository
{
    private readonly WriteDbContext _db;

    public DocumentAccessRequestRepository(WriteDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> IssueAccessInvite(AccessInvite invite, CancellationToken ct)
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

        return entity.Id;
    }

    public async Task<AccessInvite?> GetActiveInviteAsync(Guid userId, string token, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var entity = await _db.Set<DocumentAccessRequestEntity>()
            .AsNoTracking()
            .Where(x => x.UserId == userId
                        && x.RequestToken == token
                        && x.ExpiredAt > now) 
            .OrderByDescending(x => x.ExpiredAt)
            .FirstOrDefaultAsync(ct);

        if (entity is null)
            return null;

        return new AccessInvite(entity.Id, entity.UserId,
            entity.DocumentId,
            entity.RequestToken,
            entity.ExpiredAt
        );
    }

    public async Task<AccessInvite?> GetActiveInviteAsync(Guid userId, Guid documentId, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var entity = await _db.Set<DocumentAccessRequestEntity>()
            .AsNoTracking()
            .Where(x => x.UserId == userId
                        && x.DocumentId == documentId
                        && x.ExpiredAt > now) 
            .OrderByDescending(x => x.ExpiredAt)
            .FirstOrDefaultAsync(ct);

        if (entity is null)
            return null;

        return new AccessInvite(entity.Id, entity.UserId,
            entity.DocumentId,
            entity.RequestToken,
            entity.ExpiredAt
        );
    }
    public async Task<bool> ApplyUserRequestAsync(Guid id, string reason, DocumentAccessRequestType accessType, DateTime submittedDate, CancellationToken ct)
    {
        var e = await _db.Set<DocumentAccessRequestEntity>()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (e is null) return false;

        e.AccessReason = reason;
        e.AccessType = accessType;
        e.SubmittedDate = submittedDate;

        return true;
    }

}