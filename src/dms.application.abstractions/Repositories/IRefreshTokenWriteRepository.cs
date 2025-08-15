using DMS.Application.Abstractions.Persistence.Write;

namespace DMS.Application.Abstractions.Repositories;

public interface IRefreshTokenWriteRepository
{
    Task<RefreshToken?> GetAsync(string token, CancellationToken ct);
    Task AddAsync(RefreshToken token, CancellationToken ct);
    Task RotateAsync(Guid userId, string oldToken, string newToken, DateTime expires, string newParentJti, CancellationToken ct);
}