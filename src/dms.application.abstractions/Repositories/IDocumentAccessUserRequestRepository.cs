using DMS.Application.Abstractions.Persistence.Read;
using DMS.Domain.DocumentAccesses;

namespace DMS.Application.Abstractions.Repositories;

public interface IDocumentAccessUserRequestRepository
{
    Task ApplyUserRequestAsync(Guid inviteId, Guid userId, string userName, Guid documentId, string documentTitle,
        string reason, DocumentAccessRequestType accessType, DateTime requestDate,
        DocumentRequestDecisionStatus decisionStatus, Guid? decisionUserId, string? decisionUserName, string? decisionComment, DateTime? decisionDate, CancellationToken ct);
    Task<IReadOnlyCollection<DocumentAccessItem>> GetAllAsync(Guid? byUserId, CancellationToken ct);
}