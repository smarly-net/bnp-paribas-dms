using DMS.Domain.DocumentAccesses;

namespace DMS.Contracts.DocumentAccesses.List;

public sealed record DocumentAccessListItemResponseDto(Guid InviteId, string User, string DocumentTitle, string? Reason, DocumentAccessRequestType AccessType, DateTime RequestDate, DocumentRequestDecisionStatus Status, string? DecisionComment, DateTime? DecisionDate, string? Approver);