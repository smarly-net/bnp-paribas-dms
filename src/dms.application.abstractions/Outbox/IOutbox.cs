using DMS.Application.Abstractions.Persistence.Read;

namespace DMS.Application.Abstractions.Outbox;

public interface IOutbox
{
    Task EnqueueAsync<T>(T @event, CancellationToken ct);
    Task<IReadOnlyList<OutboxEnvelope>> GetPendingAsync(int take, CancellationToken ct);
    Task MarkProcessedAsync(Guid id, CancellationToken ct);
    Task<(int attempts, DateTime nextTryUtc)> MarkFailedAsync(Guid id, CancellationToken ct);
}