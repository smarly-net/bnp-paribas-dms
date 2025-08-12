namespace DMS.Infrastructure.Read.Entities;

public class UserReadEntity
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
}