namespace DMS.Contracts.Events;

public sealed record AccessInviteIssuedEvent(
    Guid UserId,
    Guid DocumentId,
    string Token,
    DateTime ExpiresAtUtc
);