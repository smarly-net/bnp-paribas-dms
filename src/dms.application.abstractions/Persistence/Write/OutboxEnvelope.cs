namespace DMS.Application.Abstractions.Persistence.Write;

public sealed record OutboxEnvelope(Guid Id, string Type, string Payload);