namespace DMS.Application.Abstractions.Persistence.Write;

public sealed record AccessInvite(Guid UserId, Guid DocumentId, string Token, DateTime ExpiresAtUtc);