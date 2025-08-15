using DMS.Application.Abstractions.Outbox;
using DMS.Application.Abstractions.Persistence.Write;
using DMS.Application.Abstractions.Repositories;
using DMS.Contracts.Events;

using System.Text.Json;
using DMS.Domain.DocumentAccesses;

namespace DMS.Infrastructure.Read.Projections;

public sealed class AccessRequestSubmittedProjector : IProjector
{
    private readonly IDocumentAccessUserRequestRepository _repo;
    public AccessRequestSubmittedProjector(IDocumentAccessUserRequestRepository repo) => _repo = repo;

    public async Task HandleAsync(OutboxEnvelope evt, CancellationToken ct)
    {
        var payload = JsonSerializer.Deserialize<AccessRequestSubmittedEvent>(evt.Payload);
        if (payload is null)
        {
            throw new InvalidOperationException("Payload deserialization failed.");
        }

        await _repo.ApplyUserRequestAsync(payload.InviteId, payload.UserId, payload.UserName, payload.DocumentId, payload.DocumentTitle, payload.Reason, payload.AccessType, payload.SubmittedAtUtc, DocumentRequestDecisionStatus.Pending, payload.DecisionUserId, payload.DecisionUserName, payload.DecisionComment, payload.DecisionDate, ct);
    }
}