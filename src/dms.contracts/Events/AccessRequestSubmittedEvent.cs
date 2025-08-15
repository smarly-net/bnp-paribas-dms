using DMS.Domain.DocumentAccesses;

namespace DMS.Contracts.Events;

public sealed record AccessRequestSubmittedEvent(
    Guid InviteId,
    Guid UserId,
    string UserName,
    Guid DocumentId,
    string DocumentTitle,
    string Reason,
    DocumentAccessRequestType AccessType,
    DateTime SubmittedAtUtc,
    Guid? DecisionUserId,
    string? DecisionUserName,
    string? DecisionComment,
    DateTime? DecisionDate
);