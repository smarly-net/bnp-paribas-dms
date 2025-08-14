using DMS.Application.Abstractions.Outbox;
using DMS.Application.Abstractions.Persistence.Write;
using DMS.Application.Abstractions.Repositories;
using DMS.Contracts.Events;
using System.Text.Json;

namespace DMS.Infrastructure.Read.Projections;

public sealed class AccessInviteIssuedProjector : IProjector
{
    private readonly IDocumentAccessInviteProjectionRepository _repo;
    public AccessInviteIssuedProjector(IDocumentAccessInviteProjectionRepository repo) => _repo = repo;

    public async Task HandleAsync(OutboxEnvelope evt, CancellationToken ct)
    {
        var payload = JsonSerializer.Deserialize<AccessInviteIssuedEvent>(evt.Payload);
        if (payload is null)
        {
            throw new InvalidOperationException("Payload deserialization failed.");
        }

        await _repo.ProjectAsync(payload.UserId, payload.DocumentId, payload.Token, payload.ExpiresAtUtc, ct);
    }
}