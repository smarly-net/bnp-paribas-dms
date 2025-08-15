using DMS.Domain.Documents;

namespace DMS.Infrastructure.Read.Entities;

public class DocumentAccessUserRequestReadEntity
{
    public Guid InviteId { get; set; }
    public Guid UserId { get; set; }
    public Guid DocumentId { get; set; }
    public string DocumentTitle { get; set; } = default!;
    public string? Reason { get; set; } = default!;
    public DocumentAccessRequestType AccessType { get; set; }
    public DateTime RequestDate { get; set; }
    public DocumentRequestDecisionStatus DecisionStatus { get; set; }
    public Guid? DecisionUserId { get; set; }
    public string? DecisionComment { get; set; }
    public DateTime? DecisionDate { get; set; }
}