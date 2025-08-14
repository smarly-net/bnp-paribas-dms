using DMS.Application.Abstractions.Persistence.Write;

namespace DMS.Application.Abstractions.Repositories;

public interface IDocumentAccessRequestRepository
{
    Task IssueAccessInvite(AccessInvite invite, CancellationToken ct);
}