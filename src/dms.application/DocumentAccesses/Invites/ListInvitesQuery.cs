using DMS.Application.Abstractions.Persistence.Read;
using DMS.Application.Common;

using MediatR;

namespace DMS.Application.DocumentAccesses.Invites;

public sealed record ListInvitesQuery(Guid UserId, string[] Roles, bool IncludeExpired) : IRequest<Result<IReadOnlyCollection<DocumentAccessInviteItem>>>;