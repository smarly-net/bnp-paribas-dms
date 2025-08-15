using DMS.Application.Abstractions.Outbox;
using DMS.Application.Abstractions.Persistence.Write;
using DMS.Contracts.Events;
using DMS.Domain.DocumentAccesses;

using System.Text.Json;
using DMS.Application.Abstractions.Notifications;

namespace DMS.Infrastructure.Read.Projections;

public sealed class AccessRequestDecidedNotificationProjector : IProjector
{
    private readonly INotificationSender _sender;
    public AccessRequestDecidedNotificationProjector(INotificationSender sender) => _sender = sender;

    public async Task HandleAsync(OutboxEnvelope evt, CancellationToken ct)
    {
        var e = JsonSerializer.Deserialize<AccessRequestDecidedEvent>(evt.Payload)
                ?? throw new InvalidOperationException("Bad payload");

        var subject = e.Status == DocumentRequestDecisionStatus.Approved
            ? "Your access request has been approved"
            : "Your access request has been rejected";

        var body = e.Status == DocumentRequestDecisionStatus.Approved
            ? $"Your request to access document {e.DocumentId} was approved at {e.DecidedAtUtc:u}."
            : $"Your request to access document {e.DocumentId} was rejected at {e.DecidedAtUtc:u}. " +
              (string.IsNullOrWhiteSpace(e.Comment) ? "" : $"Comment: {e.Comment}");

        await _sender.SendAsync(e.UserId, subject, body, ct);
    }
}