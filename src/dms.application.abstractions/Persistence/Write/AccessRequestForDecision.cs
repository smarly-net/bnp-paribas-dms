using DMS.Domain.DocumentAccesses;

namespace DMS.Application.Abstractions.Persistence.Write;

public sealed record AccessRequestForDecision(
    Guid InviteId,
    Guid UserId,
    Guid DocumentId,
    DocumentRequestDecisionStatus DecisionStatus
);