using DMS.Application.Abstractions.Persistence.Read;
using DMS.Application.Abstractions.Repositories;
using DMS.Application.Abstractions.Services;
using DMS.Infrastructure.Read.Entities;

using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Read.Repositories;

public sealed class DocumentAccessInviteRepository : IDocumentAccessInviteRepository
{
    private readonly ReadDbContext _db;
    private readonly IDateTimeService _dateTimeService;

    public DocumentAccessInviteRepository(ReadDbContext db, IDateTimeService dateTimeService)
    {
        _db = db;
        _dateTimeService = dateTimeService;
    }

    public async Task ProjectAsync(Guid inviteId, Guid userId, Guid documentId, string token, DateTime expiresAtUtc, CancellationToken ct)
    {
        var exists = await _db.DocumentAccessInvites
            .AsNoTracking()
            .AnyAsync(x => x.Token == token, ct);
        if (exists)
        {
            return;
        }

        var row = new DocumentAccessInviteReadEntity
        {
            Id = inviteId,
            UserId = userId,
            DocumentId = documentId,
            Token = token,
            ExpiresAtUtc = expiresAtUtc
        };

        _db.DocumentAccessInvites.Add(row);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyCollection<DocumentAccessInviteItem>> GetInvitesAsync(Guid? byUserId, bool includeExpired, CancellationToken ct)
    {
        var queryable = _db.DocumentAccessInvites
            .AsNoTracking();

        if (byUserId.HasValue)
        {
            queryable = queryable.Where(x => x.UserId == byUserId.Value);
        }

        if (!includeExpired)
        {
            DateTime now = _dateTimeService.UtcNow;
            queryable = queryable.Where(x => x.ExpiresAtUtc > now);
        }

        var items = await queryable
            .OrderBy(x => x.ExpiresAtUtc)
            .Select(d => new DocumentAccessInviteItem(
                d.Id
                , d.DocumentId
                , d.Token
                , d.UserId
                , d.ExpiresAtUtc))
            .ToListAsync(ct);

        return items;
    }

}