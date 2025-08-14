namespace DMS.Infrastructure.Write.Entities;

public enum DocumentRequestDecisionStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3
}
public class DocumentRequestDecisionEntity
{
    public Guid Id { get; set; }
    public Guid DocumentAccessRequestId { get; set; }
    public DocumentRequestDecisionStatus DecisionStatus { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? DecidedAtUtc { get; set; }
}