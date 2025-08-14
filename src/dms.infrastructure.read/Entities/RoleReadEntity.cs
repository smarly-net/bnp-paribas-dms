using System.ComponentModel.DataAnnotations;

namespace DMS.Infrastructure.Read.Entities;

public class RoleReadEntity
{
    public Guid Id { get; set; } 
    public string Name { get; set; } = null!;

    public ICollection<UserRoleReadEntity> UserRoles { get; set; } = new List<UserRoleReadEntity>();
}