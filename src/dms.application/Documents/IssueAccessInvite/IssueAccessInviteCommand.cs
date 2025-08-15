using DMS.Application.Abstractions.Persistence.Write;
using DMS.Application.Common;
using MediatR;

namespace DMS.Application.Documents.IssueAccessInvite;

public sealed record IssueAccessInviteCommand(
    Guid DocumentId,
    Guid UserId,
    DateTime? ExpiresAtUtc
) : IRequest<Result<AccessInvite>>;