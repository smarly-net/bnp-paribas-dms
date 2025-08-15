using DMS.Application.Abstractions.Persistence.Read;
using DMS.Application.Abstractions.Repositories;
using DMS.Application.Common;

using MediatR;

namespace DMS.Application.DocumentAccesses.Invites;

public sealed class ListInvitesQueryHandler : IRequestHandler<ListInvitesQuery, Result<IReadOnlyCollection<DocumentAccessInviteItem>>>
{
    private readonly IDocumentAccessInviteRepository _documentAccessInviteRepository;

    public ListInvitesQueryHandler(
        IDocumentAccessInviteRepository documentAccessInviteRepository)
    {
        _documentAccessInviteRepository = documentAccessInviteRepository;
    }

    public async Task<Result<IReadOnlyCollection<DocumentAccessInviteItem>>> Handle(
        ListInvitesQuery request,
        CancellationToken ct)
    {
        Guid? byUserId = null;
        bool includeExpired = request.IncludeExpired;

        switch (request.Roles)
        {
            case var r when r.Contains("admin"):
                break;
            default:
                byUserId = request.UserId;
                includeExpired = false;
                break;
        }

        var items = await _documentAccessInviteRepository.GetInvitesAsync(byUserId, includeExpired, ct);
        return Result<IReadOnlyCollection<DocumentAccessInviteItem>>.Ok(items);
    }
}