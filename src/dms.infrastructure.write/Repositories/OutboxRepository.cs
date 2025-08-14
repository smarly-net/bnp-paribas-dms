using DMS.Application.Abstractions.Outbox;
using DMS.Application.Abstractions.Persistence.Read;
using DMS.Infrastructure.Write.Entities;

using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Write.Repositories;

public class OutboxRepository : IOutbox
{
    private readonly WriteDbContext _db;
    public OutboxRepository(WriteDbContext db) => _db = db;

    public Task EnqueueAsync<T>(T ev, CancellationToken ct)
    {
        _db.OutboxMessages.Add(OutboxMessageEntity.From(ev!));
        return Task.CompletedTask; 
    }

    public async Task<IReadOnlyList<OutboxEnvelope>> GetPendingAsync(int take, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var list = await _db.OutboxMessages
            .Where(x => x.ProcessedAtUtc == null &&
                        (x.NextTryUtc == null || x.NextTryUtc <= now))
            .OrderBy(x => x.OccurredAtUtc)
            .Take(take)
            .Select(x => new OutboxEnvelope(x.Id, x.Type, x.Payload))
            .ToListAsync(ct);

        return list;
    }

    public async Task MarkProcessedAsync(Guid id, CancellationToken ct)
    {
        var entity = await _db.OutboxMessages.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return;

        entity.ProcessedAtUtc = DateTime.UtcNow;
        entity.NextTryUtc = null;

        await _db.SaveChangesAsync(ct);
    }

    public async Task<(int attempts, DateTime nextTryUtc)> MarkFailedAsync(Guid id, CancellationToken ct)
    {
        var e = await _db.OutboxMessages.FirstAsync(x => x.Id == id, ct);

        const int maxBackoffSec = 60;

        e.Attempts++;
        var delay = Math.Min(maxBackoffSec, (int)Math.Pow(2, e.Attempts));
        e.NextTryUtc = DateTime.UtcNow.AddSeconds(delay);

        await _db.SaveChangesAsync(ct);
        return (e.Attempts, e.NextTryUtc.Value);
    }
}