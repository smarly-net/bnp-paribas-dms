using DMS.Domain.DocumentAccesses;

namespace DMS.Infrastructure.Write.Entities;

public class DocumentAccessRequestEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid DocumentId { get; set; }
    public string RequestToken { get; set; } = default!;
    public DateTime ExpiredAt { get; set; }
    public string? AccessReason { get; set; }
    public DocumentAccessRequestType? AccessType { get; set; }
    public DateTime? SubmittedDate { get; set; }

    public ICollection<DocumentRequestDecisionEntity> Decisions { get; set; } = new List<DocumentRequestDecisionEntity>();
}