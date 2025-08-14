using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace DMS.Infrastructure.Write.Entities;

public sealed class OutboxMessageEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = null!;
    public string Payload { get; set; } = null!;
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAtUtc { get; set; }
    public int Attempts { get; set; }
    public DateTime? NextTryUtc { get; set; }
    public static OutboxMessageEntity From(object ev) => new()
    {
        Type = ev.GetType().Name,
        Payload = JsonSerializer.Serialize(ev)
    };
}