namespace DMS.Infrastructure.Write.Entities;

public enum DocumentAccessRequestType
{
    Read = 1,
    Edit = 2
}

public class DocumentAccessRequestEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid DocumentId { get; set; }
    public string RequestToken { get; set; } = default!;
    public string? AccessReason { get; set; }
    public DocumentAccessRequestType? AccessType { get; set; }
    public DateTime ExpiredAt { get; set; }
}