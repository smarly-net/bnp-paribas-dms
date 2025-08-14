using DMS.Application.Common;
using DMS.Contracts.Documents.Invite;

using MediatR;

namespace DMS.Application.Documents.IssueAccessInvite;

public sealed record IssueAccessInviteCommand(
    Guid DocumentId,
    Guid UserId,
    DateTime? ExpiresAtUtc
) : IRequest<Result<IssueAccessInviteResponseDto>>;