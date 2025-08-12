using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMS.Application.Abstractions.Persistence.Read;

namespace DMS.Infrastructure.Read.Repositories;

using Microsoft.EntityFrameworkCore;

public sealed class UserReadRepository : IUserReadRepository
{
    private readonly ReadDbContext _db;
    public UserReadRepository(ReadDbContext db) => _db = db;

    public async Task<UserRead?> GetByUsernameAsync(string username, CancellationToken ct)
    {
        var list = await _db.Users.ToListAsync();

        var e = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username, ct);

        return e is null ? null : new UserRead(e.Id, e.Username, e.PasswordHash);
    }
}