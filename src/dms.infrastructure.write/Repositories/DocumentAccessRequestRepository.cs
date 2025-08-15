using DMS.Application.Abstractions.Persistence.Write;
using DMS.Application.Abstractions.Repositories;
using DMS.Application.Abstractions.Services;
using DMS.Domain.DocumentAccesses;
using DMS.Infrastructure.Write.Entities;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Write.Repositories;

public sealed class DocumentAccessRequestRepository : IDocumentAccessRequestRepository
{
    private readonly WriteDbContext _db;
    private readonly IDateTimeService _dateTimeService;

    public DocumentAccessRequestRepository(WriteDbContext db, IDateTimeService dateTimeService)
    {
        _db = db;
        _dateTimeService = dateTimeService;
    }

    public async Task<Guid> IssueAccessInviteAsync(AccessInvite invite, CancellationToken ct)
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
        var now = _dateTimeService.UtcNow;

        var entity = await _db.Set<DocumentAccessRequestEntity>()
            .AsNoTracking()
            .Where(x => x.UserId == userId
                        && x.RequestToken == token
                        && (x.ExpiredAt > now || x.AccessType != null)) 
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
        var now = _dateTimeService.UtcNow;

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

        _db.DocumentRequestDecisions.Add(new
            DocumentRequestDecisionEntity
            {
                Id = Guid.NewGuid(),
                DocumentAccessRequestId = e.Id,
                DecisionStatus = DocumentRequestDecisionStatus.Pending,
                CreatedAtUtc = _dateTimeService.UtcNow,
            });

        return true;
    }

    public async Task<AccessRequestForDecision?> GetForDecisionAsync(Guid inviteId, CancellationToken ct)
    {
        var e = await _db.DocumentRequestDecisions
            .AsNoTracking()
            .Include(x => x.DocumentAccessRequest)
            .FirstOrDefaultAsync(x => x.DocumentAccessRequestId == inviteId, ct);

        if (e is null)
            return null;

        return new AccessRequestForDecision(
            e.Id,
            e.DocumentAccessRequest.UserId,
            e.DocumentAccessRequest.DocumentId,
            e.DecisionStatus);
    }

    public async Task ApplyDecisionAsync(Guid inviteId, Guid approverId, DocumentRequestDecisionStatus status, string? comment, DateTime decidedAtUtc, CancellationToken ct)
    {
        var e = await _db.DocumentRequestDecisions
            .FirstOrDefaultAsync(x => x.DocumentAccessRequestId == inviteId, ct);

        if (e is null)
        {
            throw new InvalidOperationException($"Access request {inviteId} not found.");
        }

        e.DocumentAccessRequestId = inviteId;
        e.DecisionStatus = status;
        e.ApproverUserId = approverId;
        e.Comment = comment;
        e.DecidedAtUtc = decidedAtUtc;
        e.CreatedAtUtc = _dateTimeService.UtcNow;

        _db.DocumentRequestDecisions.Update(e);
    }
}