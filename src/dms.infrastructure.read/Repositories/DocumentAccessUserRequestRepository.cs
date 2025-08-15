using DMS.Application.Abstractions.Repositories;
using DMS.Domain.Documents;
using DMS.Infrastructure.Read.Entities;

using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Read.Repositories;

public class DocumentAccessUserRequestRepository : IDocumentAccessUserRequestRepository
{
    private readonly ReadDbContext _db;
    public DocumentAccessUserRequestRepository(ReadDbContext db) => _db = db;

    public async Task ApplyUserRequestAsync(Guid inviteId, Guid userId, Guid documentId, string documentTitle, string reason,
        DocumentAccessRequestType accessType, DateTime requestDate, DocumentRequestDecisionStatus decisionStatus, CancellationToken ct)
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
            DocumentId = documentId,
            DocumentTitle = documentTitle,
            Reason = reason,
            AccessType = accessType,
            RequestDate = requestDate,
            DecisionStatus = decisionStatus
        };

        _db.DocumentAccessUserRequests.Add(entity);
        await _db.SaveChangesAsync(ct);
    }
}