using DMS.Domain.DocumentAccesses;

namespace DMS.Application.Abstractions.Persistence.Read;

public sealed record DocumentAccessItem(Guid InviteId, string UserName, string DocumentTitle, string? Reason, DocumentAccessRequestType AccessType, DateTime RequestDate, DocumentRequestDecisionStatus Status, string? DecisionComment, DateTime? DecisionDate, Guid? DecisionByUserId, string? DecisionByUserName);