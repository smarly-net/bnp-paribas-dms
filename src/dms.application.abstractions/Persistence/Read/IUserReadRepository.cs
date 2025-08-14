namespace DMS.Application.Abstractions.Persistence.Read;

public interface IUserReadRepository
{
    Task<UserRead?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<UserRead?> GetByUsernameAsync(string username, CancellationToken ct);
}