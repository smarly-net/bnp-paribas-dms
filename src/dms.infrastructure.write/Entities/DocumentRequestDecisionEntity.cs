using DMS.Domain.DocumentAccesses;

namespace DMS.Infrastructure.Write.Entities;

public class DocumentRequestDecisionEntity
{
    public Guid Id { get; set; }
    public Guid DocumentAccessRequestId { get; set; }
    public DocumentAccessRequestEntity DocumentAccessRequest { get; set; } = default!;
    public DocumentRequestDecisionStatus DecisionStatus { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? DecidedAtUtc { get; set; }
    public Guid ApproverUserId { get; set; }
}