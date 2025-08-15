namespace DMS.Application.Abstractions.Repositories;

public interface IDocumentAccessInviteProjectionRepository
{
    Task ProjectAsync(Guid inviteId, Guid userId, Guid documentId, string token, DateTime expiresAtUtc, CancellationToken ct);
}