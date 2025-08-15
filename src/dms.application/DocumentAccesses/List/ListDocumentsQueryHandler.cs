using DMS.Application.Abstractions.Persistence.Read;
using DMS.Application.Abstractions.Repositories;
using DMS.Application.Common;
using DMS.Contracts.Documents.List;

using MediatR;

using System.Data;

namespace DMS.Application.DocumentAccesses.List;

public sealed class ListDocumentAccessesQueryHandler
    : IRequestHandler<ListDocumentAccessesQuery, Result<IReadOnlyCollection<DocumentAccessItem>>>
{
    private readonly IDocumentAccessUserRequestRepository _documentAccessUserRequestRepository;

    public ListDocumentAccessesQueryHandler(
        IDocumentAccessUserRequestRepository documentAccessUserRequestAccessUserRequestRepository)
    {
        _documentAccessUserRequestRepository = documentAccessUserRequestAccessUserRequestRepository;
    }

    public async Task<Result<IReadOnlyCollection<DocumentAccessItem>>> Handle(ListDocumentAccessesQuery request, CancellationToken ct)
    {
        Guid? byUserId = null;

        switch (request.Roles)
        {
            case var r when r.Contains("admin") || r.Contains("approver"):
                break;
            default:
                byUserId = request.UserId;
                break;
        }

        var items = await _documentAccessUserRequestRepository.GetAllAsync(byUserId, ct);
        return Result<IReadOnlyCollection<DocumentAccessItem>>.Ok(items);
    }
}