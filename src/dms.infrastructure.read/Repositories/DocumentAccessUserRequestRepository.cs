using DMS.Application.Abstractions.Persistence.Read;
using DMS.Application.Abstractions.Repositories;
using DMS.Domain.DocumentAccesses;
using DMS.Infrastructure.Read.Entities;

using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Read.Repositories;

public class DocumentAccessUserRequestRepository : IDocumentAccessUserRequestRepository
{
    private readonly ReadDbContext _db;
    public DocumentAccessUserRequestRepository(ReadDbContext db) => _db = db;

    public async Task ApplyUserRequestAsync(Guid inviteId, Guid userId, string userName, Guid documentId,
        string documentTitle, string reason,
        DocumentAccessRequestType accessType, DateTime requestDate, DocumentRequestDecisionStatus decisionStatus, Guid? decisionUserId, string? decisionUserName, string? decisionComment, DateTime? decisionDate, CancellationToken ct)
    {
        var exists = await _db.DocumentAccessUserRequests
            .AsNoTracking()
            .AnyAsync(x => x.InviteId == inviteId, ct);
        if (exists)
        {
            return;
        }

        var entity = new DocumentAccessUserRequestReadEntity
        {
            InviteId = inviteId,
            UserId = userId,
            UserName = userName,
            DocumentId = documentId,
            DocumentTitle = documentTitle,
            Reason = reason,
            AccessType = accessType,
            RequestDate = requestDate,
            DecisionStatus = decisionStatus,
            DecisionUserId = decisionUserId,
            DecisionUserName = decisionUserName,
            DecisionComment = decisionComment,
            DecisionDate = decisionDate,
        };

        _db.DocumentAccessUserRequests.Add(entity);
        await _db.SaveChangesAsync(ct);
    }


    public async Task<IReadOnlyCollection<DocumentAccessItem>> GetAllAsync(Guid? byUserId, CancellationToken ct)
    {
        var queryable = _db.DocumentAccessUserRequests
            .AsNoTracking();

        if (byUserId.HasValue)
        {
            queryable = queryable.Where(x => x.UserId == byUserId.Value);
        }

        var items = await queryable
            .OrderByDescending(x => x.RequestDate)
            .Select(d => new DocumentAccessItem(
                d.InviteId
                , d.UserName
                , d.DocumentTitle
                , d.Reason
                , d.AccessType
                , d.RequestDate
                , d.DecisionStatus
                , d.DecisionComment
                , d.DecisionDate
                , d.DecisionUserId
                , d.DecisionUserName))
            .ToListAsync(ct);

        return items;
    }

    public async Task UpdateDecisionAsync(Guid inviteId, Guid decisionUserId, DocumentRequestDecisionStatus status, string? comment, DateTime decidedAtUtc, CancellationToken ct)
    {
        var entity = await _db.DocumentAccessUserRequests.FirstOrDefaultAsync(x => x.InviteId == inviteId, ct);
        if (entity is null)
            return;

        entity.DecisionUserId = decisionUserId;
        entity.DecisionStatus = status;
        entity.DecisionComment = comment;
        entity.DecisionDate = decidedAtUtc;

        await _db.SaveChangesAsync(ct);
    }
}