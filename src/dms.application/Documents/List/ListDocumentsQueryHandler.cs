using DMS.Application.Abstractions.Persistence.Read;
using DMS.Application.Abstractions.Repositories;
using DMS.Application.Common;
using DMS.Contracts.Documents.List;

using MediatR;

namespace DMS.Application.Documents.List;

public sealed class ListDocumentsQueryHandler
    : IRequestHandler<ListDocumentsQuery, Result<IReadOnlyCollection<DocumentItem>>>
{
    private readonly IDocumentRepository _documentRepository;

    public ListDocumentsQueryHandler(
        IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<Result<IReadOnlyCollection<DocumentItem>>> Handle(ListDocumentsQuery request, CancellationToken ct)
    {
        var items = await _documentRepository.GetAllAsync(ct);
        return Result<IReadOnlyCollection<DocumentItem>>.Ok(items);
    }
}