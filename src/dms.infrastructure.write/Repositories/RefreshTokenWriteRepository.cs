using DMS.Application.Abstractions.Persistence.Write;
using DMS.Application.Abstractions.Repositories;
using DMS.Infrastructure.Write.Entities;

using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Write.Repositories;

public sealed class RefreshTokenWriteRepository : IRefreshTokenWriteRepository
{
    private readonly WriteDbContext _db;
    public RefreshTokenWriteRepository(WriteDbContext db) => _db = db;

    public Task Add(RefreshToken token, CancellationToken ct)
    {
        var entity = new RefreshTokenEntity
        {
            UserId = token.UserId,
            Token = token.Token,
            ExpiryDate = token.ExpiryDate,
            ParentAccessJti = token.ParentAccessJti
        };

        _db.RefreshTokens.Add(entity);
        return Task.CompletedTask;
    }

    public async Task<RefreshToken?> GetAsync(string token, CancellationToken ct)
    {
        var e = await _db.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Token == token, ct);
        return e is null ? null : new RefreshToken(e.UserId, e.Token, e.ExpiryDate, e.ParentAccessJti);
    }

    public async Task RotateAsync(Guid userId, string oldToken, string newToken, DateTime expires, string newParentJti, CancellationToken ct)
    {
        var e = await _db
            .RefreshTokens
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Token == oldToken, ct);

        if (e is null) return;

        _db.RefreshTokens.Remove(e);
        _db.RefreshTokens.Add(new RefreshTokenEntity
        {
            UserId = userId,
            Token = newToken,
            ExpiryDate = expires,
            ParentAccessJti = newParentJti
        });
    }
}