using DMS.Domain.Documents;

namespace DMS.Contracts.Events;

public sealed record AccessRequestSubmittedEvent(
    Guid InviteId,
    Guid UserId,
    Guid DocumentId,
    string DocumentTitle,
    string Reason,
    DocumentAccessRequestType AccessType,
    DateTime SubmittedAtUtc
);