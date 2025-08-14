using System.ComponentModel.DataAnnotations;

namespace DMS.Infrastructure.Read.Entities;

public class UserReadEntity
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;

    public ICollection<UserRoleReadEntity> UserRoles { get; set; } = new List<UserRoleReadEntity>();
}