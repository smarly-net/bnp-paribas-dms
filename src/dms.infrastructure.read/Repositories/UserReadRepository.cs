using DMS.Application.Abstractions.Persistence.Read;

namespace DMS.Infrastructure.Read.Repositories;

using Microsoft.EntityFrameworkCore;

public sealed class UserReadRepository : IUserReadRepository
{
    private readonly ReadDbContext _db;
    public UserReadRepository(ReadDbContext db) => _db = db;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "CA1862")]
    public async Task<UserRead?> GetByUsernameAsync(string username, CancellationToken ct)
    {
        // In a production project, I would implement username normalization 
        // (NormalizedUsername + index) for optimal lookups without ToUpper/ToLower, 
        // but for this test task it's unnecessary.
        var uname = username.ToUpper();
        return await _db.Users
            .AsNoTracking()
            .Where(u => u.Username.ToUpper() == uname)
            .Select(u => new UserRead(
                u.Id,
                u.Username,
                u.PasswordHash,
                u.UserRoles.Select(ur => ur.Role.Name).ToArray()
            ))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<UserRead?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserRead(
                u.Id,
                u.Username,
                u.PasswordHash,
                u.UserRoles.Select(ur => ur.Role.Name).ToArray()
            ))
            .FirstOrDefaultAsync(ct);
    }
}