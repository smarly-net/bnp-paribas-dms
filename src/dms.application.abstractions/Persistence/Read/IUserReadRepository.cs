namespace DMS.Application.Abstractions.Persistence.Read;

public interface IUserReadRepository
{
    Task<UserRead?> GetByUsernameAsync(string username, CancellationToken ct);
}