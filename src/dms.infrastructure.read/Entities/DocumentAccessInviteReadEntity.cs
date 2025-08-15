namespace DMS.Infrastructure.Read.Entities;

public sealed class DocumentAccessInviteReadEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid DocumentId { get; set; }
    public string Token { get; set; } = default!;
    public DateTime ExpiresAtUtc { get; set; }

    public ICollection<DocumentAccessUserRequestReadEntity> UserRequests { get; set; } = new List<DocumentAccessUserRequestReadEntity>();
}