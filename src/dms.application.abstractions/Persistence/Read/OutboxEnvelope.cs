namespace DMS.Application.Abstractions.Persistence.Read;

public sealed record OutboxEnvelope(Guid Id, string Type, string Payload);