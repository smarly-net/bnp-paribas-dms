namespace DMS.Application.Abstractions.Persistence.Read;

public interface IProjector
{ Task HandleAsync(OutboxEnvelope message, CancellationToken ct); }