using System.ComponentModel.DataAnnotations;

namespace DMS.Infrastructure.Write.Entities;

public class RefreshTokenEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Token { get; set; } = default!;
    public DateTime ExpiryDate { get; set; }
    public string ParentAccessJti { get; set; } = default!;
}