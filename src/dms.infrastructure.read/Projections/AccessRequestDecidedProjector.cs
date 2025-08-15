using DMS.Application.Abstractions.Outbox;
using DMS.Application.Abstractions.Persistence.Write;
using DMS.Application.Abstractions.Repositories;
using DMS.Contracts.Events;

using System.Text.Json;

namespace DMS.Infrastructure.Read.Projections;

public sealed class AccessRequestDecidedProjector : IProjector
{
    private readonly IDocumentAccessUserRequestRepository _repo;

    public AccessRequestDecidedProjector(IDocumentAccessUserRequestRepository repo)
    {
        _repo = repo;
    }

    public async Task HandleAsync(OutboxEnvelope evt, CancellationToken ct)
    {
        var payload = JsonSerializer.Deserialize<AccessRequestDecidedEvent>(evt.Payload);
        if (payload is null)
            throw new InvalidOperationException("Payload deserialization failed.");

        await _repo.UpdateDecisionAsync(
            payload.InviteId,
            payload.DecisionUserId,
            payload.Status,
            payload.Comment,
            payload.DecidedAtUtc,
            ct
        );
    }
}