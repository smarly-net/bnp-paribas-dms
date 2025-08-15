using DMS.Application.Abstractions.Persistence.Read;

namespace DMS.Application.Abstractions.Repositories;

public interface IDocumentAccessInviteRepository
{
    Task ProjectAsync(Guid inviteId, Guid userId, Guid documentId, string token, DateTime expiresAtUtc, CancellationToken ct);
    Task<IReadOnlyCollection<DocumentAccessInviteItem>> GetInvitesAsync(Guid? byUserId, bool includeExpired, CancellationToken ct);
}