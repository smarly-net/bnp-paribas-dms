using DMS.Application.Abstractions.Outbox;
using DMS.Application.Abstractions.Persistence.Write;

namespace DMS.Web.Test.Service;

public sealed class NoopProjector : IProjector
{
    public Task ProjectAsync(object @event, CancellationToken ct) => Task.CompletedTask;

    public Task HandleAsync(OutboxEnvelope evt, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}