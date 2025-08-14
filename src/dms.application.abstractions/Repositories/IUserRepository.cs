using DMS.Application.Abstractions.Persistence.Read;

namespace DMS.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<IReadOnlyCollection<UserItem>> GetAllAsync(CancellationToken ct);
    Task<UserLogin?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<UserLogin?> GetByUsernameAsync(string username, CancellationToken ct);
}