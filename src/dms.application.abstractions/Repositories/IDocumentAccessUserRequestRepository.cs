using DMS.Domain.Documents;

namespace DMS.Application.Abstractions.Repositories;

public interface IDocumentAccessUserRequestRepository
{
    Task ApplyUserRequestAsync(Guid inviteId, Guid userId, Guid documentId, string documentTitle, string reason, DocumentAccessRequestType accessType, DateTime requestDate, DocumentRequestDecisionStatus decisionStatus, CancellationToken ct);
}