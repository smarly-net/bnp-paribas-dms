using DMS.Application.Abstractions.Persistence.Write;

namespace DMS.Application.Abstractions.Outbox;

public interface IProjector
{
    Task HandleAsync(OutboxEnvelope evt, CancellationToken ct);
}