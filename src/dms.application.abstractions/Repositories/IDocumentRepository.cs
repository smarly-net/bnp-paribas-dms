using DMS.Application.Abstractions.Persistence.Read;

namespace DMS.Application.Abstractions.Repositories;

public interface IDocumentRepository
{
    Task<IReadOnlyCollection<DocumentItem>> GetAllAsync(CancellationToken ct);
    Task<DocumentItem?> GetByIdAsync(Guid id, CancellationToken ct);
}