using DMS.Application.Common;
using DMS.Domain.DocumentAccesses;
using MediatR;

namespace DMS.Application.DocumentAccesses.Approve;

public sealed record ApproveAccessRequestCommand(Guid InviteId, Guid ApproverId, DocumentRequestDecisionStatus status, string? Comment)
    : IRequest<Result<Guid>>;