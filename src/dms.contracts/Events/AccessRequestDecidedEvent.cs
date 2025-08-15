using DMS.Domain.DocumentAccesses;

namespace DMS.Contracts.Events;

public sealed record AccessRequestDecidedEvent(
    Guid InviteId,
    Guid DocumentId,
    Guid UserId,
    Guid DecisionUserId,
    DocumentRequestDecisionStatus Status,
    string? Comment,
    DateTime DecidedAtUtc
);