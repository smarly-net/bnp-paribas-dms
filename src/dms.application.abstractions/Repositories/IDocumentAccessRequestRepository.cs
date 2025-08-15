using DMS.Application.Abstractions.Persistence.Write;
using DMS.Domain.DocumentAccesses;

namespace DMS.Application.Abstractions.Repositories;

public interface IDocumentAccessRequestRepository
{
    Task<Guid> IssueAccessInvite(AccessInvite invite, CancellationToken ct);
    Task<AccessInvite?> GetActiveInviteAsync(Guid userId, Guid documentId, CancellationToken ct);
    Task<AccessInvite?> GetActiveInviteAsync(Guid userId, string token, CancellationToken ct);

    Task<bool> ApplyUserRequestAsync(Guid id, string reason, DocumentAccessRequestType accessType, DateTime submittedDate, 
        CancellationToken ct);
}