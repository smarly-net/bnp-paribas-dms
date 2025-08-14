using System.Text.Json;

using DMS.Application.Abstractions.Outbox;
using DMS.Application.Abstractions.Persistence.Write;

namespace DMS.Web.Test.Service;

internal sealed class ImmediateOutbox : IOutbox
{
    private readonly IProjector _projector;
    public ImmediateOutbox(IProjector projector) => _projector = projector;

    public Task EnqueueAsync<T>(T @event, CancellationToken ct)
    {
        var env = new OutboxEnvelope(Guid.NewGuid(), typeof(T).Name, JsonSerializer.Serialize(@event));
        return _projector.HandleAsync(env, ct);
    }

    public Task<IReadOnlyList<OutboxEnvelope>> GetPendingAsync(int take, CancellationToken ct)
        => Task.FromResult<IReadOnlyList<OutboxEnvelope>>(Array.Empty<OutboxEnvelope>());
    public Task MarkProcessedAsync(Guid id, CancellationToken ct) => Task.CompletedTask;
    public Task<(int attempts, DateTime nextTryUtc)> MarkFailedAsync(Guid id, CancellationToken ct)
        => Task.FromResult((0, DateTime.UtcNow));
}