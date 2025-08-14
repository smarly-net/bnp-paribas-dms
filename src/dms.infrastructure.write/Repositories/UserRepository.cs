using DMS.Application.Abstractions.Persistence.Read;
using DMS.Application.Abstractions.Repositories;

using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Write.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly WriteDbContext _db;
    public UserRepository(WriteDbContext db) => _db = db;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "CA1862")]
    public async Task<UserLogin?> GetByUsernameAsync(string username, CancellationToken ct)
    {
        // In a production project, I would implement username normalization 
        // (NormalizedUsername + index) for optimal lookups without ToUpper/ToLower, 
        // but for this test task it's unnecessary.
        var uname = username.ToUpper();
        return await _db.Users
            .AsNoTracking()
            .Where(u => u.Username.ToUpper() == uname)
            .Select(u => new UserLogin(
                u.Id,
                u.Username,
                u.PasswordHash,
                u.UserRoles.Select(ur => ur.Role.Name).ToArray()
            ))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<UserLogin?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserLogin(
                u.Id,
                u.Username,
                u.PasswordHash,
                u.UserRoles.Select(ur => ur.Role.Name).ToArray()
            ))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyCollection<UserItem>> GetAllAsync(CancellationToken ct)
    {
        return await _db.Users
            .AsNoTracking()
            .Select(u => new UserItem(u.Id, u.Username))
            .ToListAsync(ct);
    }
}